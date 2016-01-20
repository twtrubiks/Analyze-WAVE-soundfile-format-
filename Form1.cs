using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AnalyzeWAVE
{
    public partial class Form1 : Form
    {
        string content = "";
        public Form1()
        {
            InitializeComponent();

        }
        void analysisWav(string path)
        {
            string[] arrayWav = new string[44];
            //讀取wav檔的資料
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] audioData;
            audioData = br.ReadBytes(Convert.ToInt32(fs.Length));
            print("總長度:" + audioData.Length + "\n\n");
            print("0~43是wav格式資料\n");
            for (int i = 0; i <= 43; i++) // (0~43是wav格式資料)
            {
                Console.Write(Convert.ToString(audioData[i], 16) + "  ");
                print(Convert.ToString(audioData[i], 16) + "  ");
                arrayWav[i] = Convert.ToString(audioData[i], 16);
            }
            print("\n");
            string ChunkID = arrayWav[0] + " " + arrayWav[1] + " " + arrayWav[2] + " " + arrayWav[3];
            print("\n0~3 ChunkID:" + ChunkID + "\n");

            string ChunkSize = arrayWav[4] + " " + arrayWav[5] + " " + arrayWav[6] + " " + arrayWav[7];
            print("\n4~7 ChunkSize:" + ChunkSize);
            print("\nChunkSize:" + convert(arrayWav[4], arrayWav[5], arrayWav[6], arrayWav[7]));
            print("\naudioData.Length-8 = " + (audioData.Length - 8)); //audioData.Length-8
            // 36 + SubChunk2Size = 4 + (8 + SubChunk1Size) + (8 + SubChunk2Size)
            int SubChunk1Size_int = convert(arrayWav[16], arrayWav[17], arrayWav[18], arrayWav[19]);
            int Subchunk2Size_int = convert(arrayWav[40], arrayWav[41], arrayWav[42], arrayWav[43]);
            print("\n36 + SubChunk2Size = " + (36 + Subchunk2Size_int));
            print("\n4 + (8 + SubChunk1Size) + (8 + SubChunk2Size) = " + (4 + (8 + SubChunk1Size_int) + (8 + Subchunk2Size_int)) + "\n");

            string Format = arrayWav[8] + " " + arrayWav[9] + " " + arrayWav[10] + " " + arrayWav[11] + " " + arrayWav[12] + " " + arrayWav[13] + " " + arrayWav[14] + " " + arrayWav[15];
            print("\n8~15 Format:" + Format + "\n");

            string Subchunk1Size = arrayWav[16] + " " + arrayWav[17] + " " + arrayWav[18] + " " + arrayWav[19];
            print("\n16~19 Subchunk1Size:" + Subchunk1Size);
            print("\nSubchunk1Size:" + convert(arrayWav[16], arrayWav[17], arrayWav[18], arrayWav[19]));

            string AudioFormat = arrayWav[20] + " " + arrayWav[21];
            print("\n\n20~21 AudioFormat:" + AudioFormat);
            print("\nAudioFormat:" + convert(arrayWav[20], arrayWav[21]));

            string NumChannels = arrayWav[22] + " " + arrayWav[23];
            print("\n\n22~23 NumChannels:" + NumChannels);
            int NumChannels_int = convert(arrayWav[22], arrayWav[23]);
            print("\nNumChannels:" + NumChannels_int);

            string SampleRate = arrayWav[24] + " " + arrayWav[25] + " " + arrayWav[26] + " " + arrayWav[27];
            print("\n\n24~27 SampleRate:" + SampleRate);
            int SampleRate_int = convert(arrayWav[24], arrayWav[25], arrayWav[26], arrayWav[27]);
            print("\nSampleRate:" + SampleRate_int);

            string ByteRate = arrayWav[28] + " " + arrayWav[29] + " " + arrayWav[30] + " " + arrayWav[31];
            print("\n\n28~31 ByteRate:" + ByteRate);
            print("\nByteRate:" + convert(arrayWav[28], arrayWav[29], arrayWav[30], arrayWav[31]));
            int ByteRate_int = convert(arrayWav[28], arrayWav[29], arrayWav[30], arrayWav[31]);
            int BitsPerSample_int = convert(arrayWav[34], arrayWav[35]);
            print("\nSampleRate * NumChannels * BitsPerSample/8 = " + SampleRate_int * NumChannels_int * BitsPerSample_int / 8); //SampleRate * NumChannels * BitsPerSample/8

            string BlockAlign = arrayWav[32] + " " + arrayWav[33];
            print("\n\n32~33 BlockAlign:" + BlockAlign);
            print("\nBlockAlign:" + convert(arrayWav[32], arrayWav[33]));
            print("\nNumChannels * BitsPerSample/8 = " + NumChannels_int * BitsPerSample_int / 8);  //BlockAlign  = NumChannels * BitsPerSample/8

            string BitPerSample = arrayWav[34] + " " + arrayWav[35];
            print("\n\n34~35 BitPerSample:" + BitPerSample);
            print("\nBitPerSample:" + convert(arrayWav[34], arrayWav[35]));

            string Subchunk2ID = arrayWav[36] + " " + arrayWav[37] + " " + arrayWav[38] + " " + arrayWav[39];
            print("\n\n36~39 Subchunk2ID:" + Subchunk2ID);

            string Subchunk2Size = arrayWav[40] + " " + arrayWav[41] + " " + arrayWav[42] + " " + arrayWav[43];
            print("\n\n40~43 Subchunk2Size: " + Subchunk2Size);
            print("\nSubchunk2Size:" + convert(arrayWav[40], arrayWav[41], arrayWav[42], arrayWav[43]) + "\n");
            // NumSamples * NumChannels * BitsPerSample/8 = Subchunk2Size
            print("NumSamples = " + (Subchunk2Size_int / NumChannels_int / BitsPerSample_int * 8));
            print("\n\n");

            /* count volume
            int second = audioData.Length / ByteRate_int;
            //每秒平均振幅(音量)  
            for (int count = 0; count < second; count++)
            {
                double sum = 0, volume = 0;
                for (int i = 44; i < ByteRate_int + 44; i++)  //從第44個開始是振幅(音量)  
                {
                    short vOut = BitConverter.ToInt16(audioData, i);
                    volume = Convert.ToDouble(vOut);
                    sum += Math.Abs(volume);
                }
            }
            */

            StreamWriter sw = new StreamWriter("Wav_data.txt");
            sw.WriteLine(content);
            sw.Close();
        }


        int convert(string num1, string num2, string num3, string num4)
        {
            int num1_int = Convert.ToInt32(num1, 16);
            int num2_int = Convert.ToInt32(num2, 16);
            int num3_int = Convert.ToInt32(num3, 16);
            int num4_int = Convert.ToInt32(num4, 16);
            int sum = num4_int * 256 * 256 * 256 + num3_int * 256 * 256 + num2_int * 256 + num1_int * 1;
            return sum;
        }

        int convert(string num1, string num2)
        {
            int num1_int = Convert.ToInt32(num1, 16);
            int num2_int = Convert.ToInt32(num2, 16);
            int sum = num2_int * 256 + num1_int * 1;
            return sum;
        }
        void print(string txt)
        {
            Console.WriteLine(txt);
            content += txt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Wav Files (*.wav)|*.wav";

            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Console.WriteLine("path:" + file.FileName);
                print(System.IO.Path.GetFileName(file.FileName) + "\n");
                analysisWav(file.FileName);
            }
        }
    }
}
