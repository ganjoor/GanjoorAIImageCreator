using Betalgo.Ranul.OpenAI.Managers;
using Betalgo.Ranul.OpenAI;
using Newtonsoft.Json;
using System.Net;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using static Betalgo.Ranul.OpenAI.ObjectModels.RealtimeModels.RealtimeEventTypes;

namespace GanjoorAIImageCreator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            txtAPIKey.Text = Properties.Settings.Default.APIKey;
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.APIKey = txtAPIKey.Text;
            Properties.Settings.Default.Save();

            var openAiService = new OpenAIService(new OpenAIOptions()
            {
                ApiKey = Properties.Settings.Default.APIKey,
                BaseDomain = "https://api.avalai.ir/v1"
            });

            int lastPoetId = int.Parse(txtPoetId.Text);
            using (HttpClient httpClient = new HttpClient())
            {
                for (int poetId = lastPoetId; poetId > 1; poetId--)
                {
                    txtPoetId.Text = poetId.ToString();

                    string folderMain = $"C:\\ai\\{poetId}";

                    if (Directory.Exists(folderMain))
                    {
                        continue;//another instance is processing it
                    }
                    string folder = $"C:\\ai\\{poetId}-temp";
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    
                    lblStatus.Text = "فراخوانی ...";

                    Cursor = Cursors.WaitCursor;
                    Application.DoEvents();
                    HttpResponseMessage responsePoet = await httpClient.GetAsync($"https://api.ganjoor.net/api/ganjoor/poet/{poetId}");
                    if (responsePoet.StatusCode != HttpStatusCode.OK)
                    {
                        Cursor = Cursors.Default;
                        MessageBox.Show(responsePoet.ToString());
                        return;
                    }

                    responsePoet.EnsureSuccessStatusCode();

                    dynamic poet = JsonConvert.DeserializeObject<dynamic>(await responsePoet.Content.ReadAsStringAsync());

                    int catId = (int)poet.cat.id;

                    List<int> poemlist = new List<int>();
                    await catPoems(httpClient, catId, poemlist);

                    foreach (int poemId in poemlist)
                    {
                        if (File.Exists($"{folder}\\{poetId}\\{poemId}-p.txt"))
                            continue;
                        HttpResponseMessage response = await httpClient.GetAsync($"https://api.ganjoor.net/api/ganjoor/poem/{poemId}?catInfo=false&catPoems=false&rhymes=false&recitations=false&images=false&songs=false&comments=false&verseDetails=false&navigation=false&relatedpoems=false");
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            Cursor = Cursors.Default;
                            MessageBox.Show(response.ToString());
                            return;
                        }
                        response.EnsureSuccessStatusCode();
                        dynamic poem = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                        string title = poem.title;
                        string plainText = poem.plainText;

                        if (!string.IsNullOrEmpty(plainText))
                        {
                            lblStatus.Text = title;
                            //1
                            bool hasStories = false;
                            var hasStoriesResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
                            {
                                Messages = new List<ChatMessage>
                                        {
                                            ChatMessage.FromSystem(
                                                "آیا در متن زیر داستانی وجود دارد؟ اگر داستانی وجود ندارد 0 و اگر یک یا چند داستان وجود دارد 1 برگردان.."+
                                                "منظور از داستان نقل یک ماجراست و اگر نقل یک ماجرا مثل یک گفتگو یا حادثه یا واقعهٔ تاریخی یا حماسی یا عشقی در آن نیست آن را داستان محسوب نکن. " +
                                                " توضیحات اضافی به متن اضافه نکن. فقط 0 یا 1"
                                        +
                                                    Environment.NewLine
                                                    +
                                                    title + Environment.NewLine
                                                    + plainText
                                                    ),
                                        },
                                Model = Betalgo.Ranul.OpenAI.ObjectModels.Models.Gpt_4o,
                            });
                            if (hasStoriesResult.Successful)
                            {
                                string resHasStories = hasStoriesResult.Choices.First().Message.Content;
                                if (resHasStories.Contains("اکتبر"))
                                {
                                    resHasStories = "0";
                                }
                                hasStories = resHasStories != "0";
                            }
                            if (!hasStories)
                            {
                                continue;
                            }

                            //2
                            var storyResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
                            {
                                Messages = new List<ChatMessage>
                                        {
                                            ChatMessage.FromSystem(
                                                "اگر در متن زیر داستانی وجود دارد آن را به نثر ساده بنویس. اگر چند داستان وجود دارد سعی کن همه را به طور خلاصه بیان کنی. متنت حالت نقل داستان داشته باشد و در آن نتیجه‌گیری نکن. اگر داستانی وجود ندارد خالی برگردان و هیچ توضیح اضافه‌ای مثل این که داستانی وجود ندارد نده."+
                                                "منظور از داستان نقل یک ماجراست و اگر نقل یک ماجرا مثل یک گفتگو یا حادثه یا واقعهٔ تاریخی یا حماسی یا عشقی در آن نیست آن را داستان محسوب نکن. " +
                                                " توضیحات اضافی به متن اضافه نکن."
                                                    +
                                                    Environment.NewLine
                                                    +
                                                    title + Environment.NewLine
                                                    + plainText
                                                    ),
                                        },
                                Model = Betalgo.Ranul.OpenAI.ObjectModels.Models.Gpt_4o,
                            });
                            string story = "";
                            if (storyResult.Successful)
                            {
                                story = storyResult.Choices.First().Message.Content;
                                if (story.Contains("اکتبر"))
                                {
                                    story = "";
                                }
                                story = story.Trim();
                                if (story.Split(' ').Length < 20)
                                {
                                    story = "";
                                }
                            }

                            if (string.IsNullOrEmpty(story))
                            {
                                continue;
                            }


                            //3
                            var depictionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
                            {
                                Messages = new List<ChatMessage>
                                        {
                                            ChatMessage.FromSystem(
                                                "Create a DALL-E propmt for depicting this story using an photorealistic image with Persian miniature style:"
                                                    +
                                                    Environment.NewLine
                                                    +
                                                    story
                                                    ),
                                        },
                                Model = Betalgo.Ranul.OpenAI.ObjectModels.Models.Gpt_4o,
                            });

                            string prompt = "";
                            if (depictionResult.Successful)
                            {
                                prompt = depictionResult.Choices.First().Message.Content;
                                prompt = prompt.Trim();
                            }

                            if (string.IsNullOrEmpty(prompt))
                            {

                                continue;
                            }

                            //4
                            var imageCreationResult = await openAiService.Image.CreateImage(new ImageCreateRequest
                              (
                              prompt
                              )
                            {
                                Model = "dall-e-3"
                            }
                              );
                            if (imageCreationResult.Successful)
                            {
                                string imageUrl = imageCreationResult.Results.Select(u => u.Url).First();

                                if (!string.IsNullOrEmpty(imageUrl))
                                {
                                    byte[]? imageData = null;
                                    int _ImportRetryCount = 10;
                                    int _ImportRetryInitialSleep = 500;
                                    int retryCount = 0;
                                    while (retryCount < _ImportRetryCount)
                                    {
                                        Thread.Sleep(_ImportRetryInitialSleep * (retryCount + 1));
                                        try
                                        {
                                            imageData = await httpClient.GetByteArrayAsync(imageUrl);
                                            break;
                                        }
                                        catch
                                        {
                                            retryCount++; ;
                                        }
                                    }
                                    if (imageData == null)
                                    {
                                        MessageBox.Show("imageData == null");
                                        return;
                                    }
                                    using var stream = new MemoryStream(imageData);
                                    using var originalImage = Image.FromStream(stream);
                                    originalImage.Save($"{folder}\\{poemId}.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                    File.WriteAllText($"{folder}\\{poemId}.txt", story);
                                    File.WriteAllText($"{folder}\\{poemId}-p.txt", prompt);
                                }
                            }
                        }
                    }

                    Directory.CreateDirectory(folderMain);
                    foreach(var fileName in Directory.GetFiles(folder))
                    {
                        File.Move(fileName, Path.Combine(folderMain, Path.GetFileName(fileName)));
                    }
                    Directory.Delete(folder);

                }
            }
        }

        private async Task catPoems(HttpClient httpClient, int catId, List<int> poemlist)
        {
            HttpResponseMessage response = await httpClient.GetAsync($"https://api.ganjoor.net/api/ganjoor/cat/{catId}?poems=true&mainSections=false");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(response.ToString());
                return;
            }
            response.EnsureSuccessStatusCode();
            dynamic cat = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

            foreach (dynamic poem in cat.cat.poems)
            {
                poemlist.Add((int)poem.id);
            }

            foreach (dynamic child in cat.cat.children)
            {
                await catPoems(httpClient, (int)child.id, poemlist);
            }
        }
    }
}
