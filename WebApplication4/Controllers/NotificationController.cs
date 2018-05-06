using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Services;
using WebApplication4.SignalIR;

namespace WebApplication4.Controllers
{
    public class NotificationController : Controller
    {
        [Route("notifications/", Name = "notifications")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var currentUserProfile = UserProfileService.GetUserProfileByUserId(new Guid(userId));
            var notifications = NotificationService.GetAllNotifications(currentUserProfile.Id);
            return View(notifications);
        }

        [Route("notifications/delete", Name = "notification-delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(Int16 notificationId)
        {
            NotificationService.DeleteNotification(notificationId);
            return Json(new { Message = "notification_deleted" });
        }
        [Route("notifications/delete-all", Name = "notification-delete-all")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteAll(Guid userProfileId)
        {
            NotificationService.DeleteAllNotifications(userProfileId);
            return Json(new { Message = "all_notification_deleted" });
        }
    }
}
