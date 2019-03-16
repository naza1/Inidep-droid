using Inidep2.Helper;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Inidep2
{
    public partial class MainPage : ContentPage
    {
        private static MediaFile _file;
        private static Xamarin.Essentials.Location _location;

        public MainPage()
        {
            InitializePage();
        }

        private void InitializePage()
        {
            InitializeComponent();
            ButtonSent.IsVisible = false;
            LatitudeLabel.IsVisible = false;
            LongitudeLabel.IsVisible = false;
            Email.IsVisible = false;
            EmailValidationError.IsVisible = false;
            EditorField.IsVisible = false;
        }

        //TODO: remove void and replace with Task
        private async void Button_Clicked(object sender, EventArgs e)
        {
           await CrossMedia.Current.Initialize();

           System.Threading.Thread.Sleep(2000);
           if (!CrossMedia.Current.IsTakePhotoSupported && !CrossMedia.Current.IsPickPhotoSupported)
           {
               await DisplayAlert("Message", "Photo captured", "ok");
           }
           else
           {
                await Task.Delay(100);
                var store = new StoreCameraMediaOptions
               {
                   Directory = "images",
                   CustomPhotoSize = 50,
                   SaveToAlbum = true,
                   Name = DateTime.Now + "test.jpg"
               };

               _file = await CrossMedia.Current.TakePhotoAsync(store);

               if (_file == null)
                   return;
               //await DisplayAlert("File Path",file.Path,"ok");

               MyImage.IsVisible = true;
               MyImage.Source = ImageSource.FromStream(() => _file.GetStream());

               await GetLocalization();

               Email.IsVisible = true;
               EditorField.IsVisible = true;
               ButtonSent.IsVisible = true;
           }
        }

        private bool ValidateEmail()
        {
            var email = Email.Text;

            const string emailPattern = "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)" +
                                        "(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$";

            EmailValidationError.IsVisible = true;

            if (string.IsNullOrEmpty(email))
            {
                EmailValidationError.TextColor = Color.Red;
                EmailValidationError.Text = "Complete Email field";
                return false;
            }
            if (Regex.IsMatch(email, emailPattern))
            {
                EmailValidationError.TextColor = Color.Green;
                EmailValidationError.Text = "Email is valid";
                return true;
            }

            EmailValidationError.TextColor = Color.Red;
            EmailValidationError.Text = "Email is not valid";
            return false;
        }

        private async void Button_Sent(object sender, EventArgs e)
        {
            if (!ValidateEmail()) return;

            var content = new MultipartFormDataContent
            {
                {new StreamContent(_file.GetStream()), "\"file\"", $"\"{_file.Path}\""}
            };

            var obj = new { email = Email.Text, text = EditorField.Text, latitude = _location.Latitude, longitude = _location.Longitude };
            //content.Add(new StringContent(JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8));

            try
            {
                //var responseImage = HttpHelper.CreateRequestByteContent("http://test.com", HttpMethod.Post, content.ReadAsByteArrayAsync().Result);
                //var responseData = HttpHelper.CreateRequest("https://10.54.66.160:9000/3/matching/search?list_id=3c9f2623-28be-435f-a49f-4dc29c186809&limit=1", HttpMethod.Post, obj);


                //var responseContentImage = await responseImage.Content.ReadAsStringAsync();
                //var responseContentData = await responseData.Content.ReadAsStringAsync();

                //if (responseData.StatusCode == HttpStatusCode.OK)
                //{
                //    await DisplayAlert("MobileAccessControl", $"Data: {responseContentData}", "OK");
                //}
                //else
                //{
                //    await DisplayAlert("MobileAccessControl", "Read not OK.", "OK");
                //}
            }
            catch
            {
                await DisplayAlert("Message", "Photo no sent correctly", "ok");
                InitializeComponent();
                return;
            }

            await DisplayAlert("Message", "Photo sent correctly", "ok");
            InitializePage();
        }

        private async Task GetLocalization()
        {
            try
            {
                _location = await Geolocation.GetLocationAsync();

                if (_location == null)
                    return;

                LatitudeLabel.IsVisible = true;
                LongitudeLabel.IsVisible = true;
                LatitudeLabel.Text = $"Latitude: {_location.Latitude.ToString(CultureInfo.InvariantCulture)}";
                LongitudeLabel.Text = $"Longitude: {_location.Longitude.ToString(CultureInfo.InvariantCulture)}";

                //var options = new MapLaunchOptions { Name = "Microsoft Building 25" };
                //await Map.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Message", "No get Geolocation" + ex.Message, "ok");
            }
        }
    }
}
