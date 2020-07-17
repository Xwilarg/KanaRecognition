using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KanaRecognition
{
    class Program
    {
        static void Main()
        {
            Dictionary<string, string> hiragana = JsonConvert.DeserializeObject<Dictionary<string, string>>("{\"あ\":\"a\",\"い\":\"i\",\"う\":\"u\",\"え\":\"e\",\"お\":\"o\",\"か\":\"ka\",\"き\":\"ki\",\"く\":\"ku\",\"け\":\"ke\",\"こ\":\"ko\",\"さ\":\"sa\",\"し\":\"shi\",\"す\":\"su\",\"せ\":\"se\",\"そ\":\"so\",\"た\":\"ta\",\"ち\":\"chi\",\"つ\":\"tsu\",\"て\":\"te\",\"と\":\"to\",\"な\":\"na\",\"に\":\"ni\",\"ぬ\":\"nu\",\"ね\":\"ne\",\"の\":\"no\",\"は\":\"ha\",\"ひ\":\"hi\",\"ふ\":\"fu\",\"へ\":\"he\",\"ほ\":\"ho\",\"ま\":\"ma\",\"み\":\"mi\",\"む\":\"mu\",\"め\":\"me\",\"も\":\"mo\",\"や\":\"ya\",\"ゆ\":\"yu\",\"よ\":\"yo\",\"ら\":\"ra\",\"り\":\"ri\",\"る\":\"ru\",\"れ\":\"re\",\"ろ\":\"ro\",\"わ\":\"wa\",\"を\":\"wo\",\"ん\":\"n\",\"が\":\"ga\",\"ぎ\":\"gi\",\" ぐ\":\"gu\",\"げ\":\"ge\",\"ご\":\"go\",\"ざ\":\"za\",\"ぢ\":\"ji\",\"ず\":\"zu\",\"ぜ\":\"ze\",\"ぞ\":\"zo\",\"だ\":\"da\",\"じ\":\"ji\",\"づ\":\"zu\",\"で\":\"de\",\"ど\":\"do\",\"ば\":\"ba\",\"び\":\"bi\",\"ぶ\":\"bu\",\"べ\":\"be\",\"ぼ\":\"bo\",\"ぱ\":\"pa\",\"ぴ\":\"pi\",\"ぷ\":\"pu\",\"ぺ\":\"pe\",\"ぽ\":\"po\"}");
            Dictionary<string, string> katakana = JsonConvert.DeserializeObject<Dictionary<string, string>>("{\"ア\":\"a\",\"イ\":\"i\",\"ウ\":\"u\",\"エ\":\"e\",\"オ\":\"o\",\"カ\":\"ka\",\"キ\":\"ki\",\"ク\":\"ku\",\"ケ\":\"ke\",\"コ\":\"ko\",\"サ\":\"sa\",\"シ\":\"shi\",\"ス\":\"su\",\"セ\":\"se\",\"ソ\":\"so\",\"タ\":\"ta\",\"チ\":\"chi\",\"ツ\":\"tsu\",\"テ\":\"te\",\"ト\":\"to\",\"ナ\":\"na\",\"ニ\":\"ni\",\"ヌ\":\"nu\",\"ネ\":\"ne\",\"ノ\":\"no\",\"ハ\":\"ha\",\"ヒ\":\"hi\",\"フ\":\"fu\",\"ヘ\":\"he\",\"ホ\":\"ho\",\"マ\":\"ma\",\"ミ\":\"mi\",\"ム\":\"mu\",\"メ\":\"me\",\"モ\":\"mo\",\"ヤ\":\"ya\",\"ユ\":\"yu\",\"ヨ\":\"yo\",\"ラ\":\"ra\",\"リ\":\"ri\",\"ル\":\"ru\",\"レ\":\"re\",\"ロ\":\"ro\",\"ワ\":\"wa\",\"ヲ\":\"wo\",\"ン\":\"n\",\"ガ\":\"ga\",\"ギ\":\"gi\",\" グ\":\"gu\",\"ゲ\":\"ge\",\"ゴ\":\"go\",\"ザ\":\"za\",\"ジ\":\"ji\",\"ズ\":\"zu\",\"ゼ\":\"ze\",\"ゾ\":\"zo\",\"ダ\":\"da\",\"ヂ\":\"ji\",\"ヅ\":\"zu\",\"デ\":\"de\",\"ド\":\"do\",\"バ\":\"ba\",\"ビ\":\"bi\",\"ブ\":\"bu\",\"ベ\":\"be\",\"ボ\":\"bo\",\"パ\":\"pa\",\"ピ\":\"pi\",\"プ\":\"pu\",\"ペ\":\"pe\",\"ポ\":\"po\"}"); ;

            Console.WriteLine(JsonConvert.SerializeObject(ParseDictionary(hiragana)));

            Console.WriteLine("\n\n\n\n");

            Console.WriteLine(JsonConvert.SerializeObject(ParseDictionary(katakana)));

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        private static Dictionary<string, KanaDrawInfo> ParseDictionary(Dictionary<string, string> dict)
        {
            Dictionary<string, KanaDrawInfo> infos = new Dictionary<string, KanaDrawInfo>();
            foreach (var s in dict)
            {
                using (Bitmap img = new Bitmap(30, 30))
                {
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        List<List<int>> greyScale = new List<List<int>>();
                        SizeF size = g.MeasureString(s.Key, SystemFonts.DefaultFont);
                        PointF rect = new PointF(size.Width, size.Height);

                        g.DrawString(s.Key, SystemFonts.DefaultFont, new SolidBrush(Color.Black), rect);

                        for (int y = 0; y < img.Height; y++)
                        {
                            List<int> tmp = new List<int>();
                            for (int x = 0; x < img.Width; x++)
                            {
                                tmp.Add(img.GetPixel(x, y).A == 0 ? 0 : 1);
                            }
                            greyScale.Add(tmp);
                        }

                        while (greyScale.All(x => x[0] == 0))
                        {
                            for (int i = 0; i < greyScale.Count; i++)
                                greyScale[i].RemoveAt(0);
                        }

                        while (greyScale.All(x => x[^1] == 0))
                        {
                            for (int i = 0; i < greyScale.Count; i++)
                                greyScale[i].RemoveAt(greyScale[i].Count - 1);
                        }

                        while (greyScale[0].All(x => x == 0))
                            greyScale.RemoveAt(0);

                        while (greyScale[^1].All(x => x == 0))
                            greyScale.RemoveAt(greyScale.Count - 1);

                        List<int> finalArr = new List<int>();
                        foreach (var tmp in greyScale)
                            finalArr.AddRange(tmp);

                        infos.Add(s.Key, new KanaDrawInfo
                        {
                            width = greyScale[0].Count,
                            height = greyScale.Count,
                            pixels = finalArr.ToArray()
                        });
                    }
                }
            }
            return infos;
        }
    }

    public class KanaDrawInfo
    {
        public int width;
        public int height;
        public int[] pixels;
    }
}
