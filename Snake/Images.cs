using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace snake_game
{
    //metoda statyczna jest dostępna bez konieczności tworzenia instancji obiektu danej klasy.
    public static class Images
	{
        public readonly static ImageSource Empty = Load_image("Empty.png");
        public readonly static ImageSource Body = Load_image("Body.png");
        public readonly static ImageSource Head = Load_image("Head.png");
        public readonly static ImageSource Food = Load_image("Food.png");
        public readonly static ImageSource DeadBody = Load_image("DeadBody.png");
        public readonly static ImageSource DeadHead = Load_image("DeadHead.png");

        private static ImageSource Load_image(string file_name)
        {
            return new BitmapImage(new Uri($"Assets/{file_name}", UriKind.Relative));
        }
	}
}
