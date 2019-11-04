using MimeKit;
using MimeKit.IO;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Abp.MailKit
{
    public static class EmailExtensions
    {
        /// <summary>
        /// Use MimeMessage.CreateFromMailMessage() instead
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        [Obsolete]
        public static MimeMessage ToMimeMessage(this MailMessage mail)
        {
            if (mail == null)
            {
                throw new ArgumentNullException(nameof(mail));
            }

            var headers = new List<Header>();
            foreach (var field in mail.Headers.AllKeys)
            {
                foreach (var value in mail.Headers.GetValues(field))
                {
                    headers.Add(new Header(field, value));
                }
            }

            var message = new MimeMessage(headers.ToArray());
            MimeEntity body = null;

            // Note: If the user has already sent their MailMessage via System.Net.Mail.SmtpClient,
            // then the following MailMessage properties will have been merged into the Headers, so
            // check to make sure our MimeMessage properties are empty before adding them.
            if (mail.Sender != null)
            {
                message.Sender = mail.Sender.ToMailboxAddress();
            }

            if (mail.From != null)
            {
                message.Headers.Replace(HeaderId.From, string.Empty);
                message.From.Add(mail.From.ToMailboxAddress());
            }

            if (mail.ReplyToList.Count > 0)
            {
                message.Headers.Replace(HeaderId.ReplyTo, string.Empty);
                message.ReplyTo.AddRange(mail.ReplyToList.ToInternetAddressList());
            }

            if (mail.To.Count > 0)
            {
                message.Headers.Replace(HeaderId.To, string.Empty);
                message.To.AddRange(mail.To.ToInternetAddressList());
            }

            if (mail.CC.Count > 0)
            {
                message.Headers.Replace(HeaderId.Cc, string.Empty);
                message.Cc.AddRange(mail.CC.ToInternetAddressList());
            }

            if (mail.Bcc.Count > 0)
            {
                message.Headers.Replace(HeaderId.Bcc, string.Empty);
                message.Bcc.AddRange(mail.Bcc.ToInternetAddressList());
            }

            if (mail.SubjectEncoding != null)
            {
                message.Headers.Replace(HeaderId.Subject, mail.SubjectEncoding, mail.Subject ?? string.Empty);
            }
            else
            {
                message.Subject = mail.Subject ?? string.Empty;
            }

            switch (mail.Priority)
            {
                case MailPriority.Normal:
                    message.Headers.RemoveAll(HeaderId.XMSMailPriority);
                    message.Headers.RemoveAll(HeaderId.Importance);
                    message.Headers.RemoveAll(HeaderId.XPriority);
                    message.Headers.RemoveAll(HeaderId.Priority);
                    break;
                case MailPriority.High:
                    message.Headers.Replace(HeaderId.Priority, "urgent");
                    message.Headers.Replace(HeaderId.Importance, "high");
                    message.Headers.Replace(HeaderId.XPriority, "2 (High)");
                    break;
                case MailPriority.Low:
                    message.Headers.Replace(HeaderId.Priority, "non-urgent");
                    message.Headers.Replace(HeaderId.Importance, "low");
                    message.Headers.Replace(HeaderId.XPriority, "4 (Low)");
                    break;
            }

            if (!string.IsNullOrEmpty(mail.Body))
            {
                var text = new TextPart(mail.IsBodyHtml ? "html" : "plain");
                text.SetText(mail.BodyEncoding ?? Encoding.UTF8, mail.Body);
                body = text;
            }

            if (mail.AlternateViews.Count > 0)
            {
                var alternative = new MultipartAlternative();

                if (body != null)
                {
                    alternative.Add(body);
                }

                foreach (var view in mail.AlternateViews)
                {
                    var part = GetMimePart(view);

                    if (view.BaseUri != null)
                    {
                        part.ContentLocation = view.BaseUri;
                    }

                    if (view.LinkedResources.Count > 0)
                    {
                        var type = part.ContentType.MediaType + "/" + part.ContentType.MediaSubtype;
                        var related = new MultipartRelated();

                        related.ContentType.Parameters.Add("type", type);

                        if (view.BaseUri != null)
                        {
                            related.ContentLocation = view.BaseUri;
                        }

                        related.Add(part);

                        foreach (var resource in view.LinkedResources)
                        {
                            part = GetMimePart(resource);

                            if (resource.ContentLink != null)
                                part.ContentLocation = resource.ContentLink;

                            related.Add(part);
                        }

                        alternative.Add(related);
                    }
                    else
                    {
                        alternative.Add(part);
                    }
                }

                body = alternative;
            }

            if (body == null)
            {
                body = new TextPart(mail.IsBodyHtml ? "html" : "plain");
            }

            if (mail.Attachments.Count > 0)
            {
                var mixed = new Multipart("mixed");

                if (body != null)
                {
                    mixed.Add(body);
                }

                foreach (var attachment in mail.Attachments)
                {
                    mixed.Add(GetMimePart(attachment));
                }

                body = mixed;
            }

            message.Body = body;

            return message;
        }

        private static MimePart GetMimePart(AttachmentBase item)
        {
            var mimeType = item.ContentType.ToString();
            var contentType = ContentType.Parse(mimeType);
            var attachment = item as Attachment;
            MimePart part;

            if (contentType.MediaType.Equals("text", StringComparison.OrdinalIgnoreCase))
            {
                // Original: part = new TextPart(contentType);
                // Due to constructor of TextPart(ContentType contentType) being internal, 
                // mimic the instantiation by using MimePart(ContentType contentType)
                part = new MimePart(contentType);
            }
            else
            {
                part = new MimePart(contentType);
            }

            if (attachment != null)
            {
                var disposition = attachment.ContentDisposition.ToString();
                part.ContentDisposition = ContentDisposition.Parse(disposition);
            }

            switch (item.TransferEncoding)
            {
                case System.Net.Mime.TransferEncoding.QuotedPrintable:
                    part.ContentTransferEncoding = ContentEncoding.QuotedPrintable;
                    break;
                case System.Net.Mime.TransferEncoding.Base64:
                    part.ContentTransferEncoding = ContentEncoding.Base64;
                    break;
                case System.Net.Mime.TransferEncoding.SevenBit:
                    part.ContentTransferEncoding = ContentEncoding.SevenBit;
                    break;
                case System.Net.Mime.TransferEncoding.EightBit:
                    part.ContentTransferEncoding = ContentEncoding.EightBit;
                    break;
            }

            if (item.ContentId != null)
            {
                part.ContentId = item.ContentId;
            }

            var stream = new MemoryBlockStream();
            item.ContentStream.CopyTo(stream);
            stream.Position = 0;

            part.ContentObject = new ContentObject(stream);

            return part;
        }

        /// <summary>
        /// Convert a <see cref="System.Net.Mail.MailAddress"/>
        /// to a <see cref="MailboxAddress"/>.
        /// </summary>
        /// <returns>The equivalent <see cref="MailboxAddress"/>.</returns>
        /// <param name="address">The mail address.</param>
        private static MailboxAddress ToMailboxAddress(this MailAddress address)
        {
            return address == null ? null : new MailboxAddress(address.DisplayName, address.Address);
        }

        /// <summary>
        /// Convert a <see cref="System.Net.Mail.MailAddressCollection"/>
        /// to a <see cref="InternetAddressList"/>.
        /// </summary>
        /// <returns>The equivalent <see cref="InternetAddressList"/>.</returns>
        /// <param name="addresses">The mail address.</param>
        private static InternetAddressList ToInternetAddressList(this MailAddressCollection addresses)
        {
            if (addresses == null)
            {
                return null;
            }

            var list = new InternetAddressList();
            foreach (var address in addresses)
            {
                list.Add(address.ToMailboxAddress());
            }

            return list;
        }
    }
}