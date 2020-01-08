using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models.Media;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Media;
using Nop.Services.Media;
using Nop.Web.Framework.Security;

namespace Nop.Admin.Controllers
{
    public partial class PictureController : BaseAdminController
    {
        private readonly IPictureService _pictureService;
        private readonly IRepository<Picture> _pictureRepository;

        public PictureController(IPictureService pictureService,
            IRepository<Picture> pictureRepository)
        {
            this._pictureService = pictureService;
            this._pictureRepository = pictureRepository;
        }

        [HttpPost]
        //do not validate request token (XSRF)
        [AdminAntiForgery(true)] 
        public ActionResult AsyncUpload()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
            //    return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();
            //contentType is not always available 
            //that's why we manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
            if (String.IsNullOrEmpty(contentType))
            {
                switch (fileExtension)
                {
                    case ".bmp":
                        contentType = MimeTypes.ImageBmp;
                        break;
                    case ".gif":
                        contentType = MimeTypes.ImageGif;
                        break;
                    case ".jpeg":
                    case ".jpg":
                    case ".jpe":
                    case ".jfif":
                    case ".pjpeg":
                    case ".pjp":
                        contentType = MimeTypes.ImageJpeg;
                        break;
                    case ".png":
                        contentType = MimeTypes.ImagePng;
                        break;
                    case ".tiff":
                    case ".tif":
                        contentType = MimeTypes.ImageTiff;
                        break;
                    default:
                        break;
                }
            }

            var picture = _pictureService.InsertPicture(fileBinary, contentType, null);
            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new { success = true, pictureId = picture.Id,
                imageUrl = _pictureService.GetPictureUrl(picture, 100) },
                MimeTypes.TextPlain);
        }

        public ActionResult MoveToFile()
        {
            var model = new PictureDbToFileModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult MoveToFile(PictureDbToFileModel model)
        {
            int pageIndex = 0;
            int pageSize = 500;

            while (true)
            {
                var query = from p in _pictureRepository.Table
                            orderby p.Id descending
                            where p.Id >= model.FromId && p.Id <= model.ToId
                            select p;
                var pictures = new PagedList<Picture>(query, pageIndex, pageSize);

                pageIndex++;

                if (!pictures.Any())
                    break;

                foreach (var picture in pictures)
                {
                    if (picture.PictureBinary == new byte[0] || picture.PictureBinary == null)
                        continue;

                    var pictureBinary = picture.PictureBinary;
                    string folderName = "folder_" + (picture.Id / 1000).ToString("00000");
                    SavePictureInFile(picture.Id, pictureBinary, picture.MimeType, folderName);
                }
            }
            return View(model);
        }

        protected virtual void SavePictureInFile(int pictureId, byte[] pictureBinary, string mimeType, string folderName)
        {
            string lastPart = GetFileExtensionFromMimeType(mimeType);
            string fileName = string.Format("{0}_0.{1}", pictureId.ToString("0000000"), lastPart);
            System.IO.File.WriteAllBytes(GetPictureLocalPath(fileName, folderName), pictureBinary);
        }

        protected virtual string GetFileExtensionFromMimeType(string mimeType)
        {
            if (mimeType == null)
                return null;

            //also see System.Web.MimeMapping for more mime types

            string[] parts = mimeType.Split('/');
            string lastPart = parts[parts.Length - 1];
            switch (lastPart)
            {
                case "pjpeg":
                    lastPart = "jpg";
                    break;
                case "x-png":
                    lastPart = "png";
                    break;
                case "x-icon":
                    lastPart = "ico";
                    break;
            }
            return lastPart;
        }

        protected virtual string GetPictureLocalPath(string fileName, string folderName)
        {
            var folderPath = CommonHelper.MapPath("~/content/images/" + folderName + "/");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return Path.Combine(folderPath, fileName);
        }
    }
}
