using AzStorageAccMVCAppDec12.Models;
using AzStorageAccMVCAppDec12.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AzStorageAccMVCAppDec12.Controllers
{
    public class BlobController : Controller
    {
        private readonly BlobStorageService _blobService = new BlobStorageService();
        [HttpGet]
        public ActionResult Index()
        {
            IEnumerable<BlobItemViewModel> blobs = _blobService.ListBlobs();
            return View(blobs);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                TempData["Error"] = "Please choose a file";
                return RedirectToAction("Index");
            }
            string blobName = Path.GetFileName(file.FileName);
            await _blobService.UploadAsync(blobName, file.InputStream, file.ContentType);
            TempData["Message"] = $"Uploaded '{blobName}'";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<ActionResult> Download(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new HttpStatusCodeResult(400, "Blob name is required");
            }
            Stream s = await _blobService.DownloadAsync(name);
            string contentType = "application/octet-stream";
            return File(s, contentType, name);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Blob name is required";
                return RedirectToAction("Index");
            }
            bool deleted = await _blobService.DeleteAsync(name);
            TempData["Message"] = deleted ? $"Deleted '{name}'" : $"'{name}' not found";
            return RedirectToAction("Index");
        }
    }
}