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
        public  bool CreateFriendRequest(Guid initiatorUserProfileId, Guid TargetUserProfileId)
        {
            var fr = new FriendRequest()
            {
                InitiatorUserProfileId = initiatorUserProfileId,
                TargetUserProfileId = TargetUserProfileId,
                TimeStamp = DateTime.Now,
                Used = 0
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
                Context.Entry(fr).State = EntityState.Detached;

                return false;

                throw;
            }
            return true;
        }

        public  List<FriendRequest> GetUserProfileFriendRequests(Guid userProfileId)
        {
            var result = Context.FriendRequest
                .Where(fr => fr.TargetUserProfileId == userProfileId && fr.Used == 0)
                .AsNoTracking()
                .ToList();
            return result;
        }

        public  FriendRequest GetFriendRequest(int frId)
        {
            var result = Context.FriendRequest
                .AsNoTracking()
                .FirstOrDefault(fr => fr.Id == frId);
            return result;
        }
        public  bool MarkFriendRequestAsUsed(int friendRequestId,short answer)
        {
            var entity = Context.FriendRequest.FirstOrDefault(fr => fr.Id == friendRequestId);
            if(entity != null)
            {
                entity.Used = answer;
                Context.SaveChanges();
                return true;
            }
            return false;
        }
        public  FriendRequest CheckIfFriendRequestExists(Guid initiatorUserProfileId,Guid targetUserProfileId)
        {
            var result = Context.FriendRequest
                .AsNoTracking()
                .FirstOrDefault(fr => ((fr.InitiatorUserProfileId == initiatorUserProfileId && fr.TargetUserProfileId == targetUserProfileId) ||
            (fr.InitiatorUserProfileId == targetUserProfileId && fr.TargetUserProfileId == initiatorUserProfileId)) && fr.Used == 0);
            return result;
            
        }
        public  bool DeleteFriendRequest(int friendRequestId)
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