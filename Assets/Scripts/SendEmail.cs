using System.ComponentModel;
using System.Net.Mail;
using UnityEngine;
using TMPro;

public class SendEmail : MonoBehaviour
{

    public TMP_InputField subject;
    public TMP_InputField body;
    public TMP_InputField email;
    public GameObject mainCanvas;
    public GameObject popUpPrefab;

    public void OnSendMailClick()
    {
        Send(subject.text, body.text, email.text);
    }

    public void Send(string subject, string body, string email)
    {
        if(subject == null || subject.Length == 0)
        {
            PopUpMessage popUp = Instantiate(popUpPrefab, mainCanvas.transform).GetComponent<PopUpMessage>();
            popUp.Show("Subject can not be empty.", 3);
            return;
        }
        if (body == null || body.Length == 0)
        {
            PopUpMessage popUp = Instantiate(popUpPrefab, mainCanvas.transform).GetComponent<PopUpMessage>();
            popUp.Show("Body can not be empty.", 3);
            return;
        }

        SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
        try
        {
            // Command-line argument must be the SMTP host.

            client.Credentials = new System.Net.NetworkCredential(
                "zanatomy.contact@gmail.com",// EMAIL
                "XXXXXXXX"); // PASSWORD
            client.EnableSsl = true;
            // Specify the email sender.
            // Create a mailing address that includes a UTF8 character
            // in the display name.
            MailAddress from = new MailAddress(
                "zanatomy.contact@gmail.com",
                "Z-Anatomy app support",
                System.Text.Encoding.UTF8);
            // Set destinations for the email message.
            MailAddress to = new MailAddress("zanatomy.contact@gmail.com");
            // Specify the message content.
            MailMessage message = new MailMessage(from, to);
            message.Body = "Emissor: " + email + "\n\n" + body;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = subject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            // Set the method that is called back when the send operation ends.
            client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            // The userState can be any object that allows your callback
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            string userState = "test message1";
            client.SendAsync(message, userState);
        }
        catch
        {
            PopUpMessage popUp = Instantiate(popUpPrefab, mainCanvas.transform).GetComponent<PopUpMessage>();
            popUp.Show("The message could not be sent. Please try again later.", 5);
        }
        finally
        {
            client.Dispose();
        }
      
    }

    private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        PopUpMessage popUp = Instantiate(popUpPrefab, mainCanvas.transform).GetComponent<PopUpMessage>();
        // Get the unique identifier for this asynchronous operation.
        string token = (string)e.UserState;
        if (e.Cancelled)
        {
            Debug.Log("Send canceled " + token);
        }
        if (e.Error != null)
        {
            popUp.Show("The message could not be sent. Please try again later.", 5);
            Debug.Log("[ " + token + " ] " + " " + e.Error.ToString());
        }
        else
        {
            popUp.Show("Message sent successfully", 3);
            Debug.Log("Message sent.");
        }
    }
}
