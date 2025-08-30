namespace Infrastructure.EmailTemplates;

public static class EmailVerification
{
     public static string CreateEmailVerificationTemplate(string verificationLink)
     {
         return $@"
        <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
            <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
                <h1 style='color: white; margin: 0; font-size: 28px;'>Verify Your Email</h1>
            </div>
            <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
                <p style='font-size: 16px; margin-bottom: 20px;'>Thank you for registering with <B>UserZone!</B></p>
                <p style='font-size: 16px; margin-bottom: 30px;'>Please click the button below to verify your email address:</p>
                
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{verificationLink}' style='background-color: #007bff; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-size: 16px; font-weight: bold; display: inline-block;'>Verify Email Address</a>
                </div>
                
                <p style='font-size: 14px; color: #666; margin-top: 30px;'>If you cannot click the button, copy and paste this link into your browser:</p>
                <p style='font-size: 12px; word-break: break-all; background: #e9ecef; padding: 10px; border-radius: 4px;'>{verificationLink}</p>
                
                <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
                
                <p style='font-size: 12px; color: #666;'>
                    This verification link will expire in 24 hours.<br>
                    If you didn't create an account with us, please ignore this email.
                </p>
            </div>
        </body>";
     }
}