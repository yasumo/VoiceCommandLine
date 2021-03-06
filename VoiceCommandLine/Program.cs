﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceCommandLine
{
    class Program
    {
        static private IntPtr mainWindowHandle;
        static void Main(string[] args)
        {
            initVoiceroid();
            speak(args[0]);
            //           speak("そう、御一夜に、この家にもらわれて来て、初めて知った。水が、氷が、これほどうまいということを。");
            //            speak("なら、やはり――この水を守ることもまた、僕の果たすべき義務だろう。");
        }

        static private void speak(string text) {
            // 再生ボタンハンドル
            IntPtr playBtnHWnd;

            while (true) {
                //再生ボタンが2つあればOK
                var playBtn1 = WindowHandleUtils.Operation.FindTargetButton(WindowHandleUtils.Operation.GetWindow(mainWindowHandle), " 再生", "WindowsForms10.BUTTON.app.0.378734a", 0);
                var playBtn2 = WindowHandleUtils.Operation.FindTargetButton(WindowHandleUtils.Operation.GetWindow(mainWindowHandle), " 再生", "WindowsForms10.BUTTON.app.0.378734a", 1);
                if (playBtn1 != null && playBtn2 != null)
                {
                    // 再生ボタン
                    playBtnHWnd = playBtn1.hWnd;
                    break;
                }
                System.Threading.Thread.Sleep(1000);
            }


            // テキストを入れる
            StringBuilder setTextSB = new StringBuilder(text);
            var voiceText = WindowHandleUtils.Operation.FindTargetButton(WindowHandleUtils.Operation.GetWindow(mainWindowHandle), null, "WindowsForms10.RichEdit20W.app.0.378734a", 0);
            IntPtr voiceTextHWnd = voiceText.hWnd;
            WindowHandleUtils.Operation.SendMessage(voiceTextHWnd, WindowHandleUtils.Operation.WM_SETTEXT, 0x00000000, setTextSB);
            //テキストセット前に再生ボタンを押すと駄目なので少しスリープ
            System.Threading.Thread.Sleep(200);

            //再生ボタンを押す
            WindowHandleUtils.Operation.SendMessage(playBtnHWnd, WindowHandleUtils.Operation.MK_LCLICED, WindowHandleUtils.Operation.MK_LBUTTON, 0x000A000A);
            //ボイスロイドがしゃべってる間スリープさせたい、スリープが長すぎると、AVG側のクリックができなくなるので調整が難しい・・・
            System.Threading.Thread.Sleep(500);

        }

        //ボイスロイドのEXEのウィンドウハンドルを見つけるメソッドですが、結月ゆかりEXでしか試してないです
        static private void initVoiceroid()
        {
            if (Process.GetProcessesByName("VOICEROID").Count() == 0)
            {
                //ボイスロイドが起動していなかったら起動する
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                //起動する実行ファイルのパスを設定する
                //p.StartInfo.FileName = "E:\\Program Files (x86)\\AHS\\VOICEROID+\\YukariEX\\VOICEROID.exe";
                p.StartInfo.FileName = Properties.Settings.Default.exepath;
                //起動する。プロセスが起動した時はTrueを返す。
                bool result = p.Start();
                //起動待ち時間10秒
                System.Threading.Thread.Sleep(10000);
            }

            // VoiceRoidが見つかることを前提としている
            mainWindowHandle = Process.GetProcessesByName("VOICEROID")[0].MainWindowHandle;
        }
    }
}
