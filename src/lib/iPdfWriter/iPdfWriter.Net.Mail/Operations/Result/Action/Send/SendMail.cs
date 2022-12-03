﻿
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;

using iTin.Core.ComponentModel;
using iTin.Core.ComponentModel.Results;

using iTin.Mail.Smtp.Net;

using iPdfWriter.Abstractions.Writer.Operations.Results;

using iTinIO = iTin.Core.IO;

namespace iPdfWriter.Abstractions.Writer.Operations.Actions
{
    /// <inheritdoc/>
    /// <summary>
    /// Specialization of <see cref="IOutputAction"/> interface that send the file by email.
    /// </summary>
    /// <seealso cref="IOutputAction"/>
    public class SendMail : IOutputAction
    {
        #region private constants

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string PdfExtension = "pdf";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string ZipExtension = "zip";

        #endregion

        #region interfaces

        #region IOutputAction

        #region public methods   

        /// <summary>
        /// Execute action for specified output result data.
        /// </summary>
        /// <param name="context">Target output result data.</param>
        /// <returns>
        /// <para>
        /// A <see cref="BooleanResult"/> which implements the <see cref="iTin.Core.ComponentModel.IResult{T}"/> interface reference that contains the result of the operation, to check if the operation is correct, the <b>Success</b>
        /// property will be <b>true</b> and the <b>Value</b> property will contain the value; Otherwise, the the <b>Success</b> property
        /// will be false and the <b>Errors</b> property will contain the errors associated with the operation, if they have been filled in.
        /// </para>
        /// <para>
        /// The type of the return value is <see cref="bool"/>, which contains the operation result
        /// </para>
        /// </returns>
        public IResult Execute(IOutputResultData context) => ExecuteImpl(context);

        #endregion

        #endregion

        #endregion

        #region public properties   

        /// <summary>
        /// Gets or sets the email settings
        /// </summary>
        /// <value>
        /// The email settings.
        /// </value>
        public SmtpMailSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the email settings
        /// </summary>
        /// <value>
        /// The email settings.
        /// </value>
        public string FromAddress { get; set; }

        /// <summary>
        /// Gets or sets the display name for <see cref="FromAddress"/> email address.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string FromDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the attached filename.
        /// </summary>
        /// <value>
        /// The attached filename.
        /// </value>
        public string AttachedFilename { get; set; }

        #endregion

        #region private methods

        private IResult ExecuteImpl(IOutputResultData data)
        {
            if (data == null)
            {
                return BooleanResult.NullResult;
            }

            if (Settings == null)
            {
                return BooleanResult.CreateErrorResult("Missing a valid settings");
            }

            try
            {
                var message = new MailMessage
                {
                    Subject = Settings.Templates.SubjectTemplate,
                    Body = Settings.Templates.BodyTemplate,
                    IsBodyHtml = Settings.Templates.IsBodyHtml,
                    From = new MailAddress(FromAddress, FromDisplayName),
                };

                foreach (var to in Settings.Recipients.ToAddresses)
                {
                    message.To.Add(new MailAddress(to));
                }

                foreach (var cc in Settings.Recipients.CCAddresses)
                {
                    message.CC.Add(new MailAddress(cc));
                }

                var fileExtension = data.IsZipped ? ZipExtension : PdfExtension;
                var filename = Path.ChangeExtension(AttachedFilename, fileExtension);
                message.Attachments.Add(new Attachment(data.GetOutputStream(), filename));

                foreach (var attachment in Settings.Attachments)
                {
                    var filenameNormalized = iTinIO.Path.PathResolver(attachment);
                    var fi = new FileInfo(filenameNormalized);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    message.Attachments.Add(new Attachment(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read), fi.Name));
                }

                var mail = new SmtpMail(Settings);
                mail.SendMail(message);

                return BooleanResult.SuccessResult;
            }
            catch (Exception ex)
            {
                return BooleanResult.FromException(ex);
            }
        }

        #endregion
    }
}
