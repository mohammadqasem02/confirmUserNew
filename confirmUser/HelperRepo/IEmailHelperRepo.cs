namespace confirmUser.HelperRepo
{
    public interface IEmailHelperRepo
    {
        public bool SendErrorEmail(string eNemailBody, string emailSubject, string JSonBody = null, Exception ex = null);
        public bool SendMail(string eNemailBody, string emailSubject, List<string> sentTo = null, string link = null, List<string> ccmail = null, List<string> bccmail = null, List<string> AttachmentPath = null);
    }
}
