using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Emails
{
    public static class EmailTemplates
    {
        public static string ConfirmAccount(string callbackUrl)
        {
            return $@"
            <body style='font-family: Arial, sans-serif; background-color: #f5f5f5; padding: 20px;'>
                <div style='max-width: 600px; margin: auto; background: #ffffff; padding: 30px; border-radius: 6px;'>

                    <h2 style='color: #333;'>Confirm your account</h2>

                    <p>
                        Hi,<br/><br/>
                        Thank you for registering. To complete your registration,
                        please confirm your email address by clicking the button below:
                    </p>

                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{callbackUrl}'
                           style='background-color: #4CAF50;
                                  color: white;
                                  padding: 14px 24px;
                                  text-decoration: none;
                                  font-weight: bold;
                                  border-radius: 4px;
                                  display: inline-block;'>
                            Confirm Email
                        </a>
                    </div>

                    <p style='font-size: 14px; color: #666;'>
                        If the button does not work, copy and paste the following link into your browser:
                    </p>

                    <p style='font-size: 12px; word-break: break-all;'>
                        {callbackUrl}
                    </p>

                    <p style='font-size: 14px; color: #666; margin-top: 30px;'>
                        If you did not create this account, you can safely ignore this email.
                    </p>

                </div>
            </body>";
        }
    }
}
