using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvc.StorageAccount.Demo.Data;
using Mvc.StorageAccount.Demo.Models;
using Mvc.StorageAccount.Demo.Services;

namespace Mvc.StorageAccount.Demo.Controllers
{
    public class AttendeeRegistrationController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IQueueService _queueService;
        public AttendeeRegistrationController(ITableStorageService tableStorageService, 
            IBlobStorageService blobStorageService, IQueueService queueService)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
            _queueService = queueService;
        }
        // GET: AttendeeRegistrationController
        public async Task<ActionResult> Index()
        {
            var data = await _tableStorageService.GetAttendees();
            foreach (var item in data)
            {
                item.ImageName = await _blobStorageService.GetBlobUrl(item.ImageName);
            }
            return View(data);
        }

        // GET: AttendeeRegistrationController/Details/5
        public async Task<ActionResult> Details(string id, string industry)
        {
            var data = await _tableStorageService.GetAttendee(id, industry);
            data.ImageName = await _blobStorageService.GetBlobUrl(data.ImageName);
            return View(data);
        }

        // GET: AttendeeRegistrationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AttendeeRegistrationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AttendeeEntity attendeeEntity, IFormFile formFile)
        {
            try
            {
                var id = Guid.NewGuid().ToString();
                attendeeEntity.PartitionKey = attendeeEntity.Industry;
                attendeeEntity.RowKey = Guid.NewGuid().ToString();

                if(formFile.Length > 0)
                {
                    attendeeEntity.ImageName =
                        await _blobStorageService.UploadBlob(formFile, id, attendeeEntity.ImageName);
                }
                else
                {
                    attendeeEntity.ImageName = "default.jpg";
                }
                await _tableStorageService.UpsertAttendee(attendeeEntity);

                var email = new EmailMessage
                {
                    EmailAddress = attendeeEntity.EmailAddress,
                    TimeStamp = DateTime.UtcNow,
                    Message = "Thank you for registering"
                };
                await _queueService.SendMessage(email);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendeeRegistrationController/Edit/5
        public async Task<ActionResult> Edit(string id, string industry)
        {
            AttendeeEntity data = await _tableStorageService.GetAttendee(id, industry);
            return View(data);
        }

        // POST: AttendeeRegistrationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AttendeeEntity attendeeEntity, IFormFile formFile)
        {
            try
            {
                if (formFile?.Length > 0)
                {
                    attendeeEntity.ImageName = await _blobStorageService.UploadBlob(formFile, attendeeEntity.RowKey, attendeeEntity.ImageName);
                }

                attendeeEntity.PartitionKey = attendeeEntity.Industry;
                await _tableStorageService.UpsertAttendee(attendeeEntity);

                var email = new EmailMessage
                {
                    EmailAddress = attendeeEntity.EmailAddress,
                    TimeStamp = DateTime.UtcNow,
                    Message = "Your details is successfully modified"
                };
                await _queueService.SendMessage(email);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: AttendeeRegistrationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, string industry)
        {
            try
            {
                var data = await _tableStorageService.GetAttendee(id, industry);
                await _tableStorageService.DeleteAttendee(id, industry);
                await _blobStorageService.RemoveBlob(data.ImageName);

                var email = new EmailMessage
                {
                    EmailAddress = data.EmailAddress,
                    TimeStamp = DateTime.UtcNow,
                    Message = "Your records has been deleted successfully"
                };
                await _queueService.SendMessage(email);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
