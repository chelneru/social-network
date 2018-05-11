using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class FriendRequestService : BaseService
    {
        public FriendRequestService()
        {

        }
        public static bool CreateFriendRequest(Guid initiatorUserProfileId, Guid TargetUserProfileId)
        {
            var fr = new FriendRequest()
            {
                InitiatorUserProfileId = initiatorUserProfileId,
                TargetUserProfileId = TargetUserProfileId,
                TimeStamp = DateTime.Now,
                Used = false
            };
            try
            {
                Context.FriendRequest.Add(fr);
                Context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return false;

                throw;
            }
            return true;
        }

        public static List<FriendRequest> GetUserProfileFriendRequests(Guid userProfileId)
        {
            var result = Context.FriendRequest
                .Where(fr => fr.TargetUserProfileId == userProfileId && fr.Used == false)
                .AsNoTracking()
                .ToList();
            return result;
        }

        public static bool MarkFriendRequestAsUsed(int friendRequestId)
        {
            var entity = Context.FriendRequest.FirstOrDefault(fr => fr.Id == friendRequestId);
            if(entity != null)
            {
                entity.Used = true;
                Context.SaveChanges();
                return true;
            }
            return false;
        }

        public static bool DeleteFriendRequest(int friendRequestId)
        {
            var entity = Context.FriendRequest.FirstOrDefault(fr => fr.Id == friendRequestId);
            if (entity != null)
            {
                Context.FriendRequest.Remove(entity);
                Context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}