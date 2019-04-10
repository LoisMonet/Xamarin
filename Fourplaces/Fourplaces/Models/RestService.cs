﻿using System;

using Xamarin.Forms;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TD.Api.Dtos;
using System.Text;
using System.Net.Http.Headers;
using System.Diagnostics;
using Common.Api.Dtos;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;

namespace Fourplaces.Models
{
    public class RestService
    {

        HttpClient client;
        public String url= "https://td-api.julienmialon.com/";
        public RestService()
        {
            
            Console.WriteLine("RestService");
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

        }


        public async Task<List<PlaceItemSummary>> RefreshDataAsync()
        {

            var uri = new Uri(string.Format(url+"places", string.Empty));

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Dev_Refresh:" + content);
                Response < List<PlaceItemSummary> > r = JsonConvert.DeserializeObject< Response<List<PlaceItemSummary>>>(content);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);
                return r.Data;


            }

            return null;



        }

        public async Task<PlaceItem> PlaceItemDataAsync(int id)
        {

            var uri = new Uri(string.Format(url + "places/" + id, string.Empty));

            Console.WriteLine("Dev_BefResp:");

            var response = await client.GetAsync(uri);

            Console.WriteLine("Dev_statusCode:" + response.IsSuccessStatusCode);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Response<PlaceItem> r = JsonConvert.DeserializeObject<Response<PlaceItem>>(content);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                return r.Data;
            }

            return null;

        }

        public async Task SendCommentDataAsync(int id,String comment,LoginResult lr)
        {

            var uri = new Uri(string.Format(url + "places/" + id+"/comments", string.Empty));

            Console.WriteLine("Dev_SCDBefResp:");

            CreateCommentRequest ccr = new CreateCommentRequest();
            ccr.Text = comment;
            var jsonRequest = JsonConvert.SerializeObject(ccr);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", lr.AccessToken);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Dev_SCDResponse:" + result);
            Console.WriteLine("Dev_SCDStatusCode:" + response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                Response r = JsonConvert.DeserializeObject<Response>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                //return true;
                /*if (d.is_success)
                {

                    //return d.data;
                }*/

            }
            else
            {
                Debugger.Break();

            }




        }



        public async Task<bool> SendPlaceDataAsync(String nom, String description,string lattitude,string longitude,LoginResult lr,bool camera)
        {

            var uri = new Uri(string.Format(url + "places/", string.Empty));



            double lattitudeD = double.Parse(lattitude, System.Globalization.CultureInfo.InvariantCulture);
            double longitudeD = double.Parse(longitude, System.Globalization.CultureInfo.InvariantCulture);


            CreatePlaceRequest cpr = new CreatePlaceRequest();
            cpr.Title = nom;
            cpr.Description = description;

            ImageItem iItem= await UploadImage(lr,camera);
            cpr.ImageId =iItem.Id ;

            //cpr.ImageId = 37;
            cpr.Latitude = lattitudeD;
            cpr.Longitude = longitudeD;

            Console.WriteLine("Dev_SendPlaceData:lattitudeD:" + lattitudeD + "|longitudeD:" + longitudeD+"|imageId:"+37);

            var jsonRequest = JsonConvert.SerializeObject(cpr);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", lr.AccessToken);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Dev_RDResponse:" + result);
            Console.WriteLine("Dev_RDStatusCode:" + response.StatusCode); 
            if (response.IsSuccessStatusCode)
            {
                Response r  = JsonConvert.DeserializeObject<Response>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                return true;
                /*if (d.is_success)
                {

                    //return d.data;
                }*/

            }
            else
            {
                Debugger.Break();

            }

            return false;
            //return null;



        }

        public async Task<Stream> SendPicture(bool camera)
        {
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                //Supply media options for saving our photo after it's taken.
                var pictureMediaOptions = new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Receipts",
                    Name = $"{DateTime.UtcNow}.jpg",
                    PhotoSize = PhotoSize.Small

                };

                var galleryMediaOptions = new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = PhotoSize.Small

                };

                // Take a photo of the business receipt.
                MediaFile file;
                if (camera)
                {
                    file = await CrossMedia.Current.TakePhotoAsync(pictureMediaOptions);

                }
                else{
                    file = await CrossMedia.Current.PickPhotoAsync(galleryMediaOptions);

                }
                Console.WriteLine("picture:");
                Console.WriteLine("picture:" + file);

                var stream = file.GetStream();
                file.Dispose();
                return stream;

                //return file;
            }

            return null;
        }

        private byte[] GetImageStreamAsBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public async Task<ImageItem> UploadImage(LoginResult lr,bool camera)
        {
            Stream mf=await SendPicture(camera);
            byte[] imageData = GetImageStreamAsBytes(mf);
            //byte[] imageData = await client.GetByteArrayAsync("https://bnetcmsus-a.akamaihd.net/cms/blog_header/x6/X6KQ96B3LHMY1551140875276.jpg");
            //byte[] imageData = mf.GetStream;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://td-api.julienmialon.com/images");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", lr.AccessToken);

            MultipartFormDataContent requestContent = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            // Le deuxième paramètre doit absolument être "file" ici sinon ça ne fonctionnera pas
            requestContent.Add(imageContent, "file", "file.jpg");

            request.Content = requestContent;

            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Dev_RDResponse:" + result);
            Console.WriteLine("Dev_RDStatusCode:" + response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Image uploaded!");
                Response<ImageItem> r = JsonConvert.DeserializeObject<Response<ImageItem>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                return r.Data;
            }
            else
            {
                Debugger.Break();
                return null;
            }
        }

        public async Task<LoginResult> RegisterDataAsync(String email, String fname, string lname, string mdp)
        {

            var uri = new Uri(string.Format(url + "auth/register", string.Empty));

            Console.WriteLine("Dev_RegisterData:");



            RegisterRequest rr = new RegisterRequest();
            rr.Email = email;
            rr.FirstName = fname;
            rr.LastName = lname;
            rr.Password = mdp;
            var jsonRequest = JsonConvert.SerializeObject(rr);


            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");
            //var response = client.PostAsync(uri, content).Result;


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "__access__token__");
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Dev_RDResponse:" + result);
                Response<LoginResult> r = JsonConvert.DeserializeObject<Response<LoginResult>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                if (r.IsSuccess) { 

                    return r.Data;
                }
                
            }
            else
            {
                Debugger.Break();

            }

            return null;

        }

        public async Task<LoginResult> ConnexionDataAsync(String login, string mdp)
        {

            var uri = new Uri(string.Format(url + "auth/login", string.Empty));

            Console.WriteLine("Dev_ConnexionData:");


            LoginRequest lr = new LoginRequest();
            lr.Email = login;

            lr.Password = mdp;
            var jsonRequest = JsonConvert.SerializeObject(lr);



            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");
            //var response = client.PostAsync(uri, content).Result;



            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "__access__token__");
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Dev_CDResponse:" + result);
                Response<LoginResult> r = JsonConvert.DeserializeObject<Response<LoginResult>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                if (r.IsSuccess)
                {

                    return r.Data;
                }




            }
            else
            {
                Debugger.Break();

            }

            return null;

        }

        public async Task<UserItem> UserDataAsync()
        {

            var uri = new Uri(string.Format(url + "me", string.Empty));



            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(SingletonLoginResult.LR.TokenType, SingletonLoginResult.LR.AccessToken);
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Dev_UDResponse:" + result);
                Response<UserItem> r = JsonConvert.DeserializeObject<Response<UserItem>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                if (r.IsSuccess)
                {

                    return r.Data;
                }




            }
            else
            {
                Debugger.Break();

            }

            return null;

        }

        public async Task<UserItem> EditCountAsync(String FName, string LName,int? imageId)
        {

            var uri = new Uri(string.Format(url + "me", string.Empty));

            Console.WriteLine("Dev_ECA:");


            UpdateProfileRequest upr = new UpdateProfileRequest();
            upr.FirstName = FName;
            upr.LastName = LName;
            upr.ImageId = imageId;

            var jsonRequest = JsonConvert.SerializeObject(upr);



            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");
            //var response = client.PostAsync(uri, content).Result;



            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(SingletonLoginResult.LR.TokenType, SingletonLoginResult.LR.AccessToken);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Dev_ECAesponse:" + result);
            Console.WriteLine("Dev_ECAtatusCode:" + response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Dev_ECAResponse:" + result);
                Response<UserItem> r = JsonConvert.DeserializeObject<Response<UserItem>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                if (r.IsSuccess)
                {

                    return r.Data;
                }




            }
            else
            {
                Debugger.Break();

            }

            return null;

        }

        public async Task<UserItem> EditPWAsync(String oldPW,String newPW)
        {

            var uri = new Uri(string.Format(url + "me/password", string.Empty));

            Console.WriteLine("Dev_ECA:");


            UpdatePasswordRequest upr = new UpdatePasswordRequest();
            upr.OldPassword=oldPW;
            upr.NewPassword=newPW;
            var jsonRequest = JsonConvert.SerializeObject(upr);



            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");
            //var response = client.PostAsync(uri, content).Result;



            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(SingletonLoginResult.LR.TokenType, SingletonLoginResult.LR.AccessToken);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Dev_ECAesponse:" + result);
            Console.WriteLine("Dev_ECAtatusCode:" + response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Dev_ECAResponse:" + result);
                Response<UserItem> r = JsonConvert.DeserializeObject<Response<UserItem>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                if (r.IsSuccess)
                {

                    return r.Data;
                }




            }
            else
            {
                Debugger.Break();

            }

            return null;

        }

        public async Task<ImageSource> GetRequestImage(int? id)
        {
            if (id == null)
            {
                return "profilDef.png";
            }
            else
            {
                Console.WriteLine("Dev_GetRequestImage:" + id);
                var uri = new Uri(string.Format(url + "images/" + id, string.Empty));



                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                Console.WriteLine("Dev_GetRequestImageNothing");
                Console.WriteLine("Dev_GetRequestImageGetURI:" + response.StatusCode);
                Console.WriteLine("Dev_GetRequestImageStatus:" + response.IsSuccessStatusCode);

                if (response.IsSuccessStatusCode)
                {
                    byte[] content = await response.Content.ReadAsByteArrayAsync();

                    Console.WriteLine("Dev_ImageAll:" + content);

                    ImageSource ims = ImageSource.FromStream(() => new MemoryStream(content));

                    return ims;


                }

                return "profilDef.png";
            }




        }

    }


}

