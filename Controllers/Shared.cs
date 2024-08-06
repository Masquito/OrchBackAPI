using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Orch_back_API.Entities;
using System.Drawing.Imaging;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Orch_back_API.Controllers
{
    public class Shared
    {
        public static string ImgagesFolderPath = "C:\\nginx-1.27.0\\html\\Userimages";
        private readonly MyJDBContext _context;
        private readonly IConfiguration _configuration;
        public Shared(MyJDBContext _context, IConfiguration _configuration)
        {
            this._configuration = _configuration;
            this._context = _context;
        }
        public string GenerateToken(Users user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username),
                new Claim(ClaimTypes.Role,user.Role)
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public Users Authenticate(Users userLogin)
        {
            PasswordHasher<Users> passwordHasher = new();
            var currentUser = _context.Users.FirstOrDefault(x => x.Email.ToLower() ==
                userLogin.Email.ToLower());
            if(currentUser != null)
            {
                if (passwordHasher.VerifyHashedPassword(userLogin, currentUser.Password, userLogin.Password) == PasswordVerificationResult.Success)
                {
                    return currentUser;
                }
            }
            return null;
        }

        public UInt16[] ConvertImageToUInt16Array(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int pixelCount = width * height;

            // Assuming 3 channels (RGB), 16 bits per channel (UInt16)
            UInt16[] result = new UInt16[pixelCount * 3];

            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            int bytesPerPixel = 3;
            int stride = bitmapData.Stride;
            IntPtr scan0 = bitmapData.Scan0;

            unsafe
            {
                byte* ptr = (byte*)scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = (y * stride) + (x * bytesPerPixel);
                        int resultIndex = (y * width + x) * 3;

                        byte blue = ptr[index];
                        byte green = ptr[index + 1];
                        byte red = ptr[index + 2];

                        // Assuming the conversion to UInt16 just expands the byte value to UInt16
                        result[resultIndex] = (UInt16)(red << 8);    // 16-bit red
                        result[resultIndex + 1] = (UInt16)(green << 8); // 16-bit green
                        result[resultIndex + 2] = (UInt16)(blue << 8);  // 16-bit blue
                    }
                }
            }

            bitmap.UnlockBits(bitmapData);
            return result;
        }

        public byte[] ConvertUInt16ArrayToByteArray(UInt16[] ushortArray)
        {
            int length = ushortArray.Length;
            byte[] byteArray = new byte[length * 2]; // Każdy UInt16 to dwa bajty

            for (int i = 0; i < length; i++)
            {
                byteArray[i * 2] = (byte)(ushortArray[i] & 0xFF);         // Niższy bajt
                byteArray[i * 2 + 1] = (byte)((ushortArray[i] >> 8) & 0xFF); // Wyższy bajt
            }

            return byteArray;
        }

    }
}
