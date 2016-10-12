using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTFree.Classes
{
    public class Define
    {
        /// <summary>
        /// ワークフォルダ
        /// </summary>
        public const string WorkDir    = "work";

        /// <summary>
        /// ログイン
        /// </summary>
        public const string Login      = "https://radiko.jp/ap/member/login/login";

        /// <summary>
        /// ログイン状態確認
        /// </summary>
        public const string LoginCheck = "https://radiko.jp/ap/member/webapi/member/login/check";

        /// <summary>
        /// 認証URL1
        /// </summary>
        public const string Auth1      = "https://radiko.jp/v2/api/auth1_fms";

        /// <summary>
        /// 認証URL2
        /// </summary>
        public const string Auth2      = "https://radiko.jp/v2/api/auth2_fms";

        /// <summary>
        /// swf
        /// </summary>
        public const string Swf        = "http://radiko.jp/apps/js/flash/myplayer-release.swf";

        /// <summary>
        /// m3u8
        /// </summary>
        public const string PlayList = "https://radiko.jp/v2/api/ts/playlist.m3u8?station_id={0}&ft={1}&to={2}";

        /// <summary>
        /// swfextract
        /// </summary>
        public const string SwfExtract = "swfextract";

        /// <summary>
        /// ffmpeg
        /// </summary>
        public const string Ffmpeg = "ffmpeg";

        /// <summary>
        /// ffmpeg引数
        /// </summary>
        public const string FfmpegArgs = "-i {0} -acodec copy \"{1}\".aac";

    }
}
