﻿using MessageBird.Exceptions;
using MessageBird.Objects;
using MessageBird.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;

namespace MessageBirdTests.Resources
{
    [TestClass]
    public class MessagesTests
    {
        [TestMethod]
        public void DeserializeAndSerialize()
        {
            const string CreateMessageResponseTemplate = @"{
  'id':'e7028180453e8a69d318686b17179500',
  'href':'https:\/\/rest.messagebird.com\/messages\/e7028180453e8a69d318686b17179500',
  'direction':'mt',
  'type':'sms',
  'originator':'$ORIGINATOR',
  'body':'Welcome to MessageBird',
  'reference':null,
  'validity':null,
  'gateway':56,
  'typeDetails':{
    
  },
  'datacoding':'plain',
  'mclass':1,
  'scheduledDatetime':null,
  'createdDatetime':'2014-08-11T11:18:53+00:00',
  'recipients':{
    'totalCount':1,
    'totalSentCount':1,
    'totalDeliveredCount':0,
    'totalDeliveryFailedCount':0,
    'items':[
      {
        'recipient':31612345678,
        'status':'sent',
        'statusDatetime':'2014-08-11T11:18:53+00:00'
      }
    ]
  }
}";
            Recipients recipients = new Recipients();
            Message message = new Message("", "", recipients);
            Messages messages = new Messages(message);

            messages.Deserialize(CreateMessageResponseTemplate.Replace("$ORIGINATOR", "Messagebird"));
            JsonConvert.DeserializeObject<Message>(messages.Object.ToString());

            messages.Deserialize(CreateMessageResponseTemplate.Replace("$ORIGINATOR", "3112345678"));
            JsonConvert.DeserializeObject<Message>(messages.Object.ToString());

            messages.Deserialize(CreateMessageResponseTemplate.Replace("$ORIGINATOR", "+3112345678"));
            JsonConvert.DeserializeObject<Message>(messages.Object.ToString());
        }

        [TestMethod]
        public void DeserializeRecipientsAsMsisdnsArray()
        {
            var recipients = new Recipients();
            recipients.AddRecipient(31612345678);

            var message = new Message("MsgBirdSms", "Welcome to MessageBird", recipients);
            var messages = new Messages(message);

            string serializedMessage = messages.Serialize();

            messages.Deserialize(serializedMessage);
        }

        [TestMethod]
        public void OriginatorFormat()
        {
            var recipients = new Recipients();
            recipients.AddRecipient(31612345678);

            new Message("Orignator", "This is a message from an originator", recipients);
            var message = new Message("Or igna tor", "This is a message from an originator with whitespace", recipients);
            try
            {
                message = new Message("Orignator ", "This is a message from an originator with trailing whitespace", recipients);
                Assert.Fail("Expected an error exception, because the originator contains trailing whitespace!");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
            }

            try
            {
                message = new Message(" Orignator", "This is a message from an originator with leading whitespace", recipients);
                Assert.Fail("Expected an error exception, because the originator contains leading whitespace!");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
            }
        }

    }
}
