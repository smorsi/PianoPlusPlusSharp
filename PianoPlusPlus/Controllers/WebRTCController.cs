using PianoPlusPlus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;


namespace PianoPlusPlus.Controllers
{
    public class WebRTCController : Controller
    {
        public ActionResult Index()
        {
            //return RedirectPermanent("https://www.webrtc-experiment.com/");
            return View();
        }

        // readonly WebRTCDataContext db = new WebRTCDataContext();
        PianoDBContext db = new PianoDBContext();

        /* #region Create / Join room

         [HttpPost]
         public JsonResult CreateRoom(string ownerName, string roomName, string partnerEmail = null)
         {
             if (ownerName.IsEmpty() || roomName.IsEmpty()) return Json(false);

             back:
             string token = Utility.RandomNumbers.GetRandomNumbers();
             if (db.Rooms.Any(r => r.Token == token)) goto back;

             back2:
             string ownerToken = Utility.RandomNumbers..GetRandomNumbers();
             if (db.Rooms.Any(r => r.OwnerToken == ownerToken)) goto back2;

             var room = new Room
             {
                 Token = token,
                 Name = roomName.GetValidatedString(),
                 OwnerName = ownerName.GetValidatedString(),
                 OwnerToken = ownerToken,
                 LastUpdated = DateTime.Now,
                 SharedWith = partnerEmail.IsEmpty() ? "Public" : partnerEmail,
                 Status = Status.Available
             };

             db.Rooms.InsertOnSubmit(room);
             db.SubmitChanges();

             return Json(new
             {
                 roomToken = room.Token,
                 ownerToken = room.OwnerToken
             });
         }

         [HttpPost]
         public JsonResult JoinRoom(string participant, string roomToken, string partnerEmail = null)
         {
             if (participant.IsEmpty() || roomToken.IsEmpty()) return Json(false);

             var room = db.Rooms.FirstOrDefault(r => r.Token == roomToken);
             if (room == null) return Json(false);

             if (room.SharedWith != "Public")
             {
                 if (partnerEmail.IsEmpty()) return Json(false);
                 if (room.SharedWith != partnerEmail) return Json(false);
             }

             back:
             string participantToken = Utility.RandomNumbers..GetRandomNumbers();
             if (db.Rooms.Any(r => r.OwnerToken == participantToken)) goto back;

             room.ParticipantName = participant.GetValidatedString();
             room.ParticipantToken = participantToken;
             room.LastUpdated = DateTime.Now;
             room.Status = Status.Active;

             db.SubmitChanges();

             return Json(new
             {
                 participantToken,
                 friend = room.OwnerName
             });
         }

         #endregion

         #region Search rooms

         [HttpPost]
         public JsonResult SearchPublicRooms(string partnerEmail)
         {
             if (!partnerEmail.IsEmpty()) return SearchPrivateRooms(partnerEmail);

             var rooms = db.Rooms.Where(r => r.SharedWith == "Public" && r.Status == Status.Available && r.LastUpdated.AddMinutes(1) > DateTime.Now).OrderByDescending(o => o.ID);
             return Json(
                 new
                     {
                         rooms = rooms.Select(r => new
                                                       {
                                                           roomName = r.Name,
                                                           ownerName = r.OwnerName,
                                                           roomToken = r.Token
                                                       }),
                         availableRooms = rooms.Count(),
                         publicActiveRooms= db.Rooms.Count(r => r.Status == Status.Active && r.LastUpdated.AddMinutes(1) > DateTime.Now && r.SharedWith == "Public"),
                         privateAvailableRooms = db.Rooms.Count(r => r.Status == Status.Available && r.LastUpdated.AddMinutes(1) > DateTime.Now && r.SharedWith != "Public")
                     }
                 );
         }

         [HttpPost]
         public JsonResult SearchPrivateRooms(string partnerEmail)
         {
             if (partnerEmail.IsEmpty()) return Json(false);

             var rooms = db.Rooms.Where(r => r.SharedWith == partnerEmail && r.Status == Status.Available && r.LastUpdated.AddMinutes(1) > DateTime.Now).OrderByDescending(o => o.ID);
             return Json(new
                             {
                                 rooms = rooms.Select(r => new
                                                               {
                                                                   roomName = r.Name,
                                                                   ownerName = r.OwnerName,
                                                                   roomToken = r.Token
                                                               })
                             });
         }

         #endregion

         #region SDP Messages

         [HttpPost]
         public JsonResult PostSDP(string sdp, string roomToken, string userToken)
         {
             if (sdp.IsEmpty() || roomToken.IsEmpty() || userToken.IsEmpty()) return Json(false);

             var sdpMessage = new SDPMessage
             {
                 SDP = sdp,
                 IsProcessed = false,
                 RoomToken = roomToken,
                 Sender = userToken
             };

             db.SDPMessages.InsertOnSubmit(sdpMessage);
             db.SubmitChanges();

             return Json(true);
         }

         [HttpPost]
         public JsonResult GetSDP(string roomToken, string userToken)
         {
              if (roomToken.IsEmpty() || userToken.IsEmpty()) return Json(false);

             var sdp = db.SDPMessages.FirstOrDefault(s => s.RoomToken == roomToken && s.Sender != userToken && !s.IsProcessed);

             if(sdp == null) return Json(false);

             sdp.IsProcessed = true;
             db.SubmitChanges();

             return Json(new
             {
                 sdp = sdp.SDP
             });
         }

         #endregion

         #region ICE Candidates

         [HttpPost]
         public JsonResult PostICE(string candidate, string label, string roomToken, string userToken)
         {
             if (candidate.IsEmpty() || label.IsEmpty() || roomToken.IsEmpty() || userToken.IsEmpty()) return Json(false);

             var candidateTable = new CandidatesTable
             {
                 Candidate = candidate,
                 Label = label,
                 IsProcessed = false,
                 RoomToken = roomToken,
                 Sender = userToken
             };

             db.CandidatesTables.InsertOnSubmit(candidateTable);
             db.SubmitChanges();

             return Json(true);
         }

         [HttpPost]
         public JsonResult GetICE(string roomToken, string userToken)
         {
             if (roomToken.IsEmpty() || userToken.IsEmpty()) return Json(false);

             var candidate = db.CandidatesTables.FirstOrDefault(c => c.RoomToken == roomToken && c.Sender != userToken && !c.IsProcessed);
             if (candidate == null) return Json(false);

             candidate.IsProcessed = true;
             db.SubmitChanges();

             return Json(new
             {
                 candidate = candidate.Candidate,
                 label = candidate.Label
             });
         }

         #endregion

         #region Extras

         [HttpPost]
         public JsonResult GetParticipant(string roomToken, string ownerToken)
         {
             if (roomToken.IsEmpty() || ownerToken.IsEmpty()) return Json(false);

             var room = db.Rooms.FirstOrDefault(r => r.Token == roomToken && r.OwnerToken == ownerToken);
             if (room == null) return Json(false);

             room.LastUpdated = DateTime.Now;
             db.SubmitChanges();

             if (room.ParticipantName.IsEmpty()) return Json(false);
             return Json(new { participant = room.ParticipantName });
         }

         [HttpPost]
         public JsonResult Stats()
         {
             var numberOfRooms = db.Rooms.Count();
             var numberOfPublicRooms = db.Rooms.Count(r => r.SharedWith == "Public");
             var numberOfPrivateRooms = db.Rooms.Count(r => r.SharedWith != "Public");
             var numberOfEmptyRooms = db.Rooms.Count(r => r.ParticipantName == null);
             var numberOfFullRooms = db.Rooms.Count(r => r.ParticipantName != null);
             return Json(new { numberOfRooms, numberOfPublicRooms, numberOfPrivateRooms, numberOfEmptyRooms, numberOfFullRooms });
         }

         #endregion
     }
     struct Status
     {
         public const string Available = "Available";
         public const string Active = "Active";
     } */
    }
}