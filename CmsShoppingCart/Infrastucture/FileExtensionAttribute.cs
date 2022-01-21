using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Infrastucture
{
    public class FileExtensionAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //var context = (CmsShoppingCartContext)validationContext.GetService(typeof(CmsShoppingCartContext));

            var file = value as IFormFile;

            if (file != null) 
            {
                var extention = Path.GetExtension(file.FileName);

                string[] extentions = { "jpg", "png" };

                bool result = extentions.Any(x => extention.EndsWith(x));

                if (!result) 
                {
                    return new ValidationResult(GetErrorMessage());
                }

            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return "Allowed extentions are jpg and png";
        }
    }
}
