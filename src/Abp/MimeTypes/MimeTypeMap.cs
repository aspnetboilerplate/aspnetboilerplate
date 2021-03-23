// Taken from https://github.com/samuelneff/MimeTypeMap

using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;

namespace Abp.MimeTypes
{
    public class MimeTypeMap : IMimeTypeMap, ITransientDependency
    {
        protected const string Dot = ".";
        protected const string QuestionMark = "?";

        protected readonly Lazy<IDictionary<string, string>> MappingDictionary;

        public MimeTypeMap()
        {
            MappingDictionary = new Lazy<IDictionary<string, string>>(BuildMappings);
        }

        protected virtual Dictionary<string, string> BuildMappings()
        {
            var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                #region Big freaking list of mime types

                // maps both ways,
                // extension -> mime type
                //   and
                // mime type -> extension
                //
                // any mime types on left side not pre-loaded on right side, are added automatically
                // some mime types can map to multiple extensions, so to get a deterministic mapping,
                // add those to the dictionary specifically
                //
                // combination of values from Windows 7 Registry and 
                // from C:\Windows\System32\inetsrv\config\applicationHost.config
                // some added, including .7z and .dat
                //
                // Some added based on http://www.iana.org/assignments/media-types/media-types.xhtml
                // which lists mime types, but not extensions
                //
                {".323", "text/h323"},
                {".3g2", "video/3gpp2"},
                {".3gp", "video/3gpp"},
                {".3gp2", "video/3gpp2"},
                {".3gpp", "video/3gpp"},
                {".7z", "application/x-7z-compressed"},
                {".aa", "audio/audible"},
                {".AAC", "audio/aac"},
                {".aaf", "application/octet-stream"},
                {".aax", "audio/vnd.audible.aax"},
                {".ac3", "audio/ac3"},
                {".aca", "application/octet-stream"},
                {".accda", "application/msaccess.addin"},
                {".accdb", "application/msaccess"},
                {".accdc", "application/msaccess.cab"},
                {".accde", "application/msaccess"},
                {".accdr", "application/msaccess.runtime"},
                {".accdt", "application/msaccess"},
                {".accdw", "application/msaccess.webapplication"},
                {".accft", "application/msaccess.ftemplate"},
                {".acx", "application/internet-property-stream"},
                {".AddIn", "text/xml"},
                {".ade", "application/msaccess"},
                {".adobebridge", "application/x-bridge-url"},
                {".adp", "application/msaccess"},
                {".ADT", "audio/vnd.dlna.adts"},
                {".ADTS", "audio/aac"},
                {".afm", "application/octet-stream"},
                {".ai", "application/postscript"},
                {".aif", "audio/aiff"},
                {".aifc", "audio/aiff"},
                {".aiff", "audio/aiff"},
                {".air", "application/vnd.adobe.air-application-installer-package+zip"},
                {".amc", "application/mpeg"},
                {".anx", "application/annodex"},
                {".apk", "application/vnd.android.package-archive"},
                {".apng", "image/apng"},
                {".application", "application/x-ms-application"},
                {".art", "image/x-jg"},
                {".asa", "application/xml"},
                {".asax", "application/xml"},
                {".ascx", "application/xml"},
                {".asd", "application/octet-stream"},
                {".asf", "video/x-ms-asf"},
                {".ashx", "application/xml"},
                {".asi", "application/octet-stream"},
                {".asm", "text/plain"},
                {".asmx", "application/xml"},
                {".aspx", "application/xml"},
                {".asr", "video/x-ms-asf"},
                {".asx", "video/x-ms-asf"},
                {".atom", "application/atom+xml"},
                {".au", "audio/basic"},
                {".avci", "image/avci"},
                {".avcs", "image/avcs"},
                {".avi", "video/x-msvideo"},
                {".avif", "image/avif"},
                {".avifs", "image/avif-sequence"},
                {".axa", "audio/annodex"},
                {".axs", "application/olescript"},
                {".axv", "video/annodex"},
                {".bas", "text/plain"},
                {".bcpio", "application/x-bcpio"},
                {".bin", "application/octet-stream"},
                {".bmp", "image/bmp"},
                {".c", "text/plain"},
                {".cab", "application/octet-stream"},
                {".caf", "audio/x-caf"},
                {".calx", "application/vnd.ms-office.calx"},
                {".cat", "application/vnd.ms-pki.seccat"},
                {".cc", "text/plain"},
                {".cd", "text/plain"},
                {".cdda", "audio/aiff"},
                {".cdf", "application/x-cdf"},
                {".cer", "application/x-x509-ca-cert"},
                {".cfg", "text/plain"},
                {".chm", "application/octet-stream"},
                {".class", "application/x-java-applet"},
                {".clp", "application/x-msclip"},
                {".cmd", "text/plain"},
                {".cmx", "image/x-cmx"},
                {".cnf", "text/plain"},
                {".cod", "image/cis-cod"},
                {".config", "application/xml"},
                {".contact", "text/x-ms-contact"},
                {".coverage", "application/xml"},
                {".cpio", "application/x-cpio"},
                {".cpp", "text/plain"},
                {".crd", "application/x-mscardfile"},
                {".crl", "application/pkix-crl"},
                {".crt", "application/x-x509-ca-cert"},
                {".cs", "text/plain"},
                {".csdproj", "text/plain"},
                {".csh", "application/x-csh"},
                {".csproj", "text/plain"},
                {".css", "text/css"},
                {".csv", "text/csv"},
                {".cur", "application/octet-stream"},
                {".czx", "application/x-czx"},
                {".cxx", "text/plain"},
                {".dat", "application/octet-stream"},
                {".datasource", "application/xml"},
                {".dbproj", "text/plain"},
                {".dcr", "application/x-director"},
                {".def", "text/plain"},
                {".deploy", "application/octet-stream"},
                {".der", "application/x-x509-ca-cert"},
                {".dgml", "application/xml"},
                {".dib", "image/bmp"},
                {".dif", "video/x-dv"},
                {".dir", "application/x-director"},
                {".disco", "text/xml"},
                {".divx", "video/divx"},
                {".dll", "application/x-msdownload"},
                {".dll.config", "text/xml"},
                {".dlm", "text/dlm"},
                {".doc", "application/msword"},
                {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".dot", "application/msword"},
                {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
                {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
                {".dsp", "application/octet-stream"},
                {".dsw", "text/plain"},
                {".dtd", "text/xml"},
                {".dtsConfig", "text/xml"},
                {".dv", "video/x-dv"},
                {".dvi", "application/x-dvi"},
                {".dwf", "drawing/x-dwf"},
                {".dwg", "application/acad"},
                {".dwp", "application/octet-stream"},
                {".dxf", "application/x-dxf"},
                {".dxr", "application/x-director"},
                {".eml", "message/rfc822"},
                {".emf", "image/emf"},
                {".emz", "application/octet-stream"},
                {".eot", "application/vnd.ms-fontobject"},
                {".eps", "application/postscript"},
                {".es", "application/ecmascript"},
                {".etl", "application/etl"},
                {".etx", "text/x-setext"},
                {".evy", "application/envoy"},
                {".exe", "application/vnd.microsoft.portable-executable"},
                {".exe.config", "text/xml"},
                {".f4v", "video/mp4"},
                {".fdf", "application/vnd.fdf"},
                {".fif", "application/fractals"},
                {".filters", "application/xml"},
                {".fla", "application/octet-stream"},
                {".flac", "audio/flac"},
                {".flr", "x-world/x-vrml"},
                {".flv", "video/x-flv"},
                {".fsscript", "application/fsharp-script"},
                {".fsx", "application/fsharp-script"},
                {".generictest", "application/xml"},
                {".gif", "image/gif"},
                {".gpx", "application/gpx+xml"},
                {".group", "text/x-ms-group"},
                {".gsm", "audio/x-gsm"},
                {".gtar", "application/x-gtar"},
                {".gz", "application/x-gzip"},
                {".h", "text/plain"},
                {".hdf", "application/x-hdf"},
                {".hdml", "text/x-hdml"},
                {".heic", "image/heic"},
                {".heics", "image/heic-sequence"},
                {".heif", "image/heif"},
                {".heifs", "image/heif-sequence"},
                {".hhc", "application/x-oleobject"},
                {".hhk", "application/octet-stream"},
                {".hhp", "application/octet-stream"},
                {".hlp", "application/winhlp"},
                {".hpp", "text/plain"},
                {".hqx", "application/mac-binhex40"},
                {".hta", "application/hta"},
                {".htc", "text/x-component"},
                {".htm", "text/html"},
                {".html", "text/html"},
                {".htt", "text/webviewhtml"},
                {".hxa", "application/xml"},
                {".hxc", "application/xml"},
                {".hxd", "application/octet-stream"},
                {".hxe", "application/xml"},
                {".hxf", "application/xml"},
                {".hxh", "application/octet-stream"},
                {".hxi", "application/octet-stream"},
                {".hxk", "application/xml"},
                {".hxq", "application/octet-stream"},
                {".hxr", "application/octet-stream"},
                {".hxs", "application/octet-stream"},
                {".hxt", "text/html"},
                {".hxv", "application/xml"},
                {".hxw", "application/octet-stream"},
                {".hxx", "text/plain"},
                {".i", "text/plain"},
                {".ical", "text/calendar"},
                {".icalendar", "text/calendar"},
                {".ico", "image/x-icon"},
                {".ics", "text/calendar"},
                {".idl", "text/plain"},
                {".ief", "image/ief"},
                {".ifb", "text/calendar"},
                {".iii", "application/x-iphone"},
                {".inc", "text/plain"},
                {".inf", "application/octet-stream"},
                {".ini", "text/plain"},
                {".inl", "text/plain"},
                {".ins", "application/x-internet-signup"},
                {".ipa", "application/x-itunes-ipa"},
                {".ipg", "application/x-itunes-ipg"},
                {".ipproj", "text/plain"},
                {".ipsw", "application/x-itunes-ipsw"},
                {".iqy", "text/x-ms-iqy"},
                {".isp", "application/x-internet-signup"},
                {".isma", "application/octet-stream"},
                {".ismv", "application/octet-stream"},
                {".ite", "application/x-itunes-ite"},
                {".itlp", "application/x-itunes-itlp"},
                {".itms", "application/x-itunes-itms"},
                {".itpc", "application/x-itunes-itpc"},
                {".IVF", "video/x-ivf"},
                {".jar", "application/java-archive"},
                {".java", "application/octet-stream"},
                {".jck", "application/liquidmotion"},
                {".jcz", "application/liquidmotion"},
                {".jfif", "image/pjpeg"},
                {".jnlp", "application/x-java-jnlp-file"},
                {".jpb", "application/octet-stream"},
                {".jpe", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".js", "application/javascript"},
                {".json", "application/json"},
                {".jsx", "text/jscript"},
                {".jsxbin", "text/plain"},
                {".latex", "application/x-latex"},
                {".library-ms", "application/windows-library+xml"},
                {".lit", "application/x-ms-reader"},
                {".loadtest", "application/xml"},
                {".lpk", "application/octet-stream"},
                {".lsf", "video/x-la-asf"},
                {".lst", "text/plain"},
                {".lsx", "video/x-la-asf"},
                {".lzh", "application/octet-stream"},
                {".m13", "application/x-msmediaview"},
                {".m14", "application/x-msmediaview"},
                {".m1v", "video/mpeg"},
                {".m2t", "video/vnd.dlna.mpeg-tts"},
                {".m2ts", "video/vnd.dlna.mpeg-tts"},
                {".m2v", "video/mpeg"},
                {".m3u", "audio/x-mpegurl"},
                {".m3u8", "audio/x-mpegurl"},
                {".m4a", "audio/m4a"},
                {".m4b", "audio/m4b"},
                {".m4p", "audio/m4p"},
                {".m4r", "audio/x-m4r"},
                {".m4v", "video/x-m4v"},
                {".mac", "image/x-macpaint"},
                {".mak", "text/plain"},
                {".man", "application/x-troff-man"},
                {".manifest", "application/x-ms-manifest"},
                {".map", "text/plain"},
                {".master", "application/xml"},
                {".mbox", "application/mbox"},
                {".mda", "application/msaccess"},
                {".mdb", "application/x-msaccess"},
                {".mde", "application/msaccess"},
                {".mdp", "application/octet-stream"},
                {".me", "application/x-troff-me"},
                {".mfp", "application/x-shockwave-flash"},
                {".mht", "message/rfc822"},
                {".mhtml", "message/rfc822"},
                {".mid", "audio/mid"},
                {".midi", "audio/mid"},
                {".mix", "application/octet-stream"},
                {".mk", "text/plain"},
                {".mk3d", "video/x-matroska-3d"},
                {".mka", "audio/x-matroska"},
                {".mkv", "video/x-matroska"},
                {".mmf", "application/x-smaf"},
                {".mno", "text/xml"},
                {".mny", "application/x-msmoney"},
                {".mod", "video/mpeg"},
                {".mov", "video/quicktime"},
                {".movie", "video/x-sgi-movie"},
                {".mp2", "video/mpeg"},
                {".mp2v", "video/mpeg"},
                {".mp3", "audio/mpeg"},
                {".mp4", "video/mp4"},
                {".mp4v", "video/mp4"},
                {".mpa", "video/mpeg"},
                {".mpe", "video/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpf", "application/vnd.ms-mediapackage"},
                {".mpg", "video/mpeg"},
                {".mpp", "application/vnd.ms-project"},
                {".mpv2", "video/mpeg"},
                {".mqv", "video/quicktime"},
                {".ms", "application/x-troff-ms"},
                {".msg", "application/vnd.ms-outlook"},
                {".msi", "application/octet-stream"},
                {".mso", "application/octet-stream"},
                {".mts", "video/vnd.dlna.mpeg-tts"},
                {".mtx", "application/xml"},
                {".mvb", "application/x-msmediaview"},
                {".mvc", "application/x-miva-compiled"},
                {".mxf", "application/mxf"},
                {".mxp", "application/x-mmxp"},
                {".nc", "application/x-netcdf"},
                {".nsc", "video/x-ms-asf"},
                {".nws", "message/rfc822"},
                {".ocx", "application/octet-stream"},
                {".oda", "application/oda"},
                {".odb", "application/vnd.oasis.opendocument.database"},
                {".odc", "application/vnd.oasis.opendocument.chart"},
                {".odf", "application/vnd.oasis.opendocument.formula"},
                {".odg", "application/vnd.oasis.opendocument.graphics"},
                {".odh", "text/plain"},
                {".odi", "application/vnd.oasis.opendocument.image"},
                {".odl", "text/plain"},
                {".odm", "application/vnd.oasis.opendocument.text-master"},
                {".odp", "application/vnd.oasis.opendocument.presentation"},
                {".ods", "application/vnd.oasis.opendocument.spreadsheet"},
                {".odt", "application/vnd.oasis.opendocument.text"},
                {".oga", "audio/ogg"},
                {".ogg", "audio/ogg"},
                {".ogv", "video/ogg"},
                {".ogx", "application/ogg"},
                {".one", "application/onenote"},
                {".onea", "application/onenote"},
                {".onepkg", "application/onenote"},
                {".onetmp", "application/onenote"},
                {".onetoc", "application/onenote"},
                {".onetoc2", "application/onenote"},
                {".opus", "audio/ogg"},
                {".orderedtest", "application/xml"},
                {".osdx", "application/opensearchdescription+xml"},
                {".otf", "application/font-sfnt"},
                {".otg", "application/vnd.oasis.opendocument.graphics-template"},
                {".oth", "application/vnd.oasis.opendocument.text-web"},
                {".otp", "application/vnd.oasis.opendocument.presentation-template"},
                {".ots", "application/vnd.oasis.opendocument.spreadsheet-template"},
                {".ott", "application/vnd.oasis.opendocument.text-template"},
                {".oxps", "application/oxps"},
                {".oxt", "application/vnd.openofficeorg.extension"},
                {".p10", "application/pkcs10"},
                {".p12", "application/x-pkcs12"},
                {".p7b", "application/x-pkcs7-certificates"},
                {".p7c", "application/pkcs7-mime"},
                {".p7m", "application/pkcs7-mime"},
                {".p7r", "application/x-pkcs7-certreqresp"},
                {".p7s", "application/pkcs7-signature"},
                {".pbm", "image/x-portable-bitmap"},
                {".pcast", "application/x-podcast"},
                {".pct", "image/pict"},
                {".pcx", "application/octet-stream"},
                {".pcz", "application/octet-stream"},
                {".pdf", "application/pdf"},
                {".pfb", "application/octet-stream"},
                {".pfm", "application/octet-stream"},
                {".pfx", "application/x-pkcs12"},
                {".pgm", "image/x-portable-graymap"},
                {".pic", "image/pict"},
                {".pict", "image/pict"},
                {".pkgdef", "text/plain"},
                {".pkgundef", "text/plain"},
                {".pko", "application/vnd.ms-pki.pko"},
                {".pls", "audio/scpls"},
                {".pma", "application/x-perfmon"},
                {".pmc", "application/x-perfmon"},
                {".pml", "application/x-perfmon"},
                {".pmr", "application/x-perfmon"},
                {".pmw", "application/x-perfmon"},
                {".png", "image/png"},
                {".pnm", "image/x-portable-anymap"},
                {".pnt", "image/x-macpaint"},
                {".pntg", "image/x-macpaint"},
                {".pnz", "image/png"},
                {".pot", "application/vnd.ms-powerpoint"},
                {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
                {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
                {".ppa", "application/vnd.ms-powerpoint"},
                {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
                {".ppm", "image/x-portable-pixmap"},
                {".pps", "application/vnd.ms-powerpoint"},
                {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
                {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".prf", "application/pics-rules"},
                {".prm", "application/octet-stream"},
                {".prx", "application/octet-stream"},
                {".ps", "application/postscript"},
                {".psc1", "application/PowerShell"},
                {".psd", "application/octet-stream"},
                {".psess", "application/xml"},
                {".psm", "application/octet-stream"},
                {".psp", "application/octet-stream"},
                {".pst", "application/vnd.ms-outlook"},
                {".pub", "application/x-mspublisher"},
                {".pwz", "application/vnd.ms-powerpoint"},
                {".qht", "text/x-html-insertion"},
                {".qhtm", "text/x-html-insertion"},
                {".qt", "video/quicktime"},
                {".qti", "image/x-quicktime"},
                {".qtif", "image/x-quicktime"},
                {".qtl", "application/x-quicktimeplayer"},
                {".qxd", "application/octet-stream"},
                {".ra", "audio/x-pn-realaudio"},
                {".ram", "audio/x-pn-realaudio"},
                {".rar", "application/x-rar-compressed"},
                {".ras", "image/x-cmu-raster"},
                {".rat", "application/rat-file"},
                {".rc", "text/plain"},
                {".rc2", "text/plain"},
                {".rct", "text/plain"},
                {".rdlc", "application/xml"},
                {".reg", "text/plain"},
                {".resx", "application/xml"},
                {".rf", "image/vnd.rn-realflash"},
                {".rgb", "image/x-rgb"},
                {".rgs", "text/plain"},
                {".rm", "application/vnd.rn-realmedia"},
                {".rmi", "audio/mid"},
                {".rmp", "application/vnd.rn-rn_music_package"},
                {".rmvb", "application/vnd.rn-realmedia-vbr"},
                {".roff", "application/x-troff"},
                {".rpm", "audio/x-pn-realaudio-plugin"},
                {".rqy", "text/x-ms-rqy"},
                {".rtf", "application/rtf"},
                {".rtx", "text/richtext"},
                {".rvt", "application/octet-stream"},
                {".ruleset", "application/xml"},
                {".s", "text/plain"},
                {".safariextz", "application/x-safari-safariextz"},
                {".scd", "application/x-msschedule"},
                {".scr", "text/plain"},
                {".sct", "text/scriptlet"},
                {".sd2", "audio/x-sd2"},
                {".sdp", "application/sdp"},
                {".sea", "application/octet-stream"},
                {".searchConnector-ms", "application/windows-search-connector+xml"},
                {".setpay", "application/set-payment-initiation"},
                {".setreg", "application/set-registration-initiation"},
                {".settings", "application/xml"},
                {".sgimb", "application/x-sgimb"},
                {".sgml", "text/sgml"},
                {".sh", "application/x-sh"},
                {".shar", "application/x-shar"},
                {".shtml", "text/html"},
                {".sit", "application/x-stuffit"},
                {".sitemap", "application/xml"},
                {".skin", "application/xml"},
                {".skp", "application/x-koan"},
                {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
                {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
                {".slk", "application/vnd.ms-excel"},
                {".sln", "text/plain"},
                {".slupkg-ms", "application/x-ms-license"},
                {".smd", "audio/x-smd"},
                {".smi", "application/octet-stream"},
                {".smx", "audio/x-smd"},
                {".smz", "audio/x-smd"},
                {".snd", "audio/basic"},
                {".snippet", "application/xml"},
                {".snp", "application/octet-stream"},
                {".sql", "application/sql"},
                {".sol", "text/plain"},
                {".sor", "text/plain"},
                {".spc", "application/x-pkcs7-certificates"},
                {".spl", "application/futuresplash"},
                {".spx", "audio/ogg"},
                {".src", "application/x-wais-source"},
                {".srf", "text/plain"},
                {".SSISDeploymentManifest", "text/xml"},
                {".ssm", "application/streamingmedia"},
                {".sst", "application/vnd.ms-pki.certstore"},
                {".stl", "application/vnd.ms-pki.stl"},
                {".sv4cpio", "application/x-sv4cpio"},
                {".sv4crc", "application/x-sv4crc"},
                {".svc", "application/xml"},
                {".svg", "image/svg+xml"},
                {".swf", "application/x-shockwave-flash"},
                {".step", "application/step"},
                {".stp", "application/step"},
                {".t", "application/x-troff"},
                {".tar", "application/x-tar"},
                {".tcl", "application/x-tcl"},
                {".testrunconfig", "application/xml"},
                {".testsettings", "application/xml"},
                {".tex", "application/x-tex"},
                {".texi", "application/x-texinfo"},
                {".texinfo", "application/x-texinfo"},
                {".tgz", "application/x-compressed"},
                {".thmx", "application/vnd.ms-officetheme"},
                {".thn", "application/octet-stream"},
                {".tif", "image/tiff"},
                {".tiff", "image/tiff"},
                {".tlh", "text/plain"},
                {".tli", "text/plain"},
                {".toc", "application/octet-stream"},
                {".tr", "application/x-troff"},
                {".trm", "application/x-msterminal"},
                {".trx", "application/xml"},
                {".ts", "video/vnd.dlna.mpeg-tts"},
                {".tsv", "text/tab-separated-values"},
                {".ttf", "application/font-sfnt"},
                {".tts", "video/vnd.dlna.mpeg-tts"},
                {".txt", "text/plain"},
                {".u32", "application/octet-stream"},
                {".uls", "text/iuls"},
                {".user", "text/plain"},
                {".ustar", "application/x-ustar"},
                {".vb", "text/plain"},
                {".vbdproj", "text/plain"},
                {".vbk", "video/mpeg"},
                {".vbproj", "text/plain"},
                {".vbs", "text/vbscript"},
                {".vcf", "text/x-vcard"},
                {".vcproj", "application/xml"},
                {".vcs", "text/plain"},
                {".vcxproj", "application/xml"},
                {".vddproj", "text/plain"},
                {".vdp", "text/plain"},
                {".vdproj", "text/plain"},
                {".vdx", "application/vnd.ms-visio.viewer"},
                {".vml", "text/xml"},
                {".vscontent", "application/xml"},
                {".vsct", "text/xml"},
                {".vsd", "application/vnd.visio"},
                {".vsi", "application/ms-vsi"},
                {".vsix", "application/vsix"},
                {".vsixlangpack", "text/xml"},
                {".vsixmanifest", "text/xml"},
                {".vsmdi", "application/xml"},
                {".vspscc", "text/plain"},
                {".vss", "application/vnd.visio"},
                {".vsscc", "text/plain"},
                {".vssettings", "text/xml"},
                {".vssscc", "text/plain"},
                {".vst", "application/vnd.visio"},
                {".vstemplate", "text/xml"},
                {".vsto", "application/x-ms-vsto"},
                {".vsw", "application/vnd.visio"},
                {".vsx", "application/vnd.visio"},
                {".vtt", "text/vtt"},
                {".vtx", "application/vnd.visio"},
                {".wasm", "application/wasm"},
                {".wav", "audio/wav"},
                {".wave", "audio/wav"},
                {".wax", "audio/x-ms-wax"},
                {".wbk", "application/msword"},
                {".wbmp", "image/vnd.wap.wbmp"},
                {".wcm", "application/vnd.ms-works"},
                {".wdb", "application/vnd.ms-works"},
                {".wdp", "image/vnd.ms-photo"},
                {".webarchive", "application/x-safari-webarchive"},
                {".webm", "video/webm"},
                {".webp", "image/webp"}, /* https://en.wikipedia.org/wiki/WebP */
                {".webtest", "application/xml"},
                {".wiq", "application/xml"},
                {".wiz", "application/msword"},
                {".wks", "application/vnd.ms-works"},
                {".WLMP", "application/wlmoviemaker"},
                {".wlpginstall", "application/x-wlpg-detect"},
                {".wlpginstall3", "application/x-wlpg3-detect"},
                {".wm", "video/x-ms-wm"},
                {".wma", "audio/x-ms-wma"},
                {".wmd", "application/x-ms-wmd"},
                {".wmf", "application/x-msmetafile"},
                {".wml", "text/vnd.wap.wml"},
                {".wmlc", "application/vnd.wap.wmlc"},
                {".wmls", "text/vnd.wap.wmlscript"},
                {".wmlsc", "application/vnd.wap.wmlscriptc"},
                {".wmp", "video/x-ms-wmp"},
                {".wmv", "video/x-ms-wmv"},
                {".wmx", "video/x-ms-wmx"},
                {".wmz", "application/x-ms-wmz"},
                {".woff", "application/font-woff"},
                {".woff2", "application/font-woff2"},
                {".wpl", "application/vnd.ms-wpl"},
                {".wps", "application/vnd.ms-works"},
                {".wri", "application/x-mswrite"},
                {".wrl", "x-world/x-vrml"},
                {".wrz", "x-world/x-vrml"},
                {".wsc", "text/scriptlet"},
                {".wsdl", "text/xml"},
                {".wvx", "video/x-ms-wvx"},
                {".x", "application/directx"},
                {".xaf", "x-world/x-vrml"},
                {".xaml", "application/xaml+xml"},
                {".xap", "application/x-silverlight-app"},
                {".xbap", "application/x-ms-xbap"},
                {".xbm", "image/x-xbitmap"},
                {".xdr", "text/plain"},
                {".xht", "application/xhtml+xml"},
                {".xhtml", "application/xhtml+xml"},
                {".xla", "application/vnd.ms-excel"},
                {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
                {".xlc", "application/vnd.ms-excel"},
                {".xld", "application/vnd.ms-excel"},
                {".xlk", "application/vnd.ms-excel"},
                {".xll", "application/vnd.ms-excel"},
                {".xlm", "application/vnd.ms-excel"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
                {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".xlt", "application/vnd.ms-excel"},
                {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
                {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
                {".xlw", "application/vnd.ms-excel"},
                {".xml", "text/xml"},
                {".xmp", "application/octet-stream"},
                {".xmta", "application/xml"},
                {".xof", "x-world/x-vrml"},
                {".XOML", "text/plain"},
                {".xpm", "image/x-xpixmap"},
                {".xps", "application/vnd.ms-xpsdocument"},
                {".xrm-ms", "text/xml"},
                {".xsc", "application/xml"},
                {".xsd", "text/xml"},
                {".xsf", "text/xml"},
                {".xsl", "text/xml"},
                {".xslt", "text/xml"},
                {".xsn", "application/octet-stream"},
                {".xss", "application/xml"},
                {".xspf", "application/xspf+xml"},
                {".xtp", "application/octet-stream"},
                {".xwd", "image/x-xwindowdump"},
                {".z", "application/x-compress"},
                {".zip", "application/zip"},

                {"application/fsharp-script", ".fsx"},
                {"application/msaccess", ".adp"},
                {"application/msword", ".doc"},
                {"application/octet-stream", ".bin"},
                {"application/onenote", ".one"},
                {"application/postscript", ".eps"},
                {"application/step", ".step"},
                {"application/vnd.ms-excel", ".xls"},
                {"application/vnd.ms-powerpoint", ".ppt"},
                {"application/vnd.ms-works", ".wks"},
                {"application/vnd.visio", ".vsd"},
                {"application/x-director", ".dir"},
                {"application/x-msdos-program", ".exe"},
                {"application/x-shockwave-flash", ".swf"},
                {"application/x-x509-ca-cert", ".cer"},
                {"application/x-zip-compressed", ".zip"},
                {"application/xhtml+xml", ".xhtml"},
                {"application/xml", ".xml"}, // anomaly, .xml -> text/xml, but application/xml -> many things, but all are xml, so safest is .xml
                {"audio/aac", ".AAC"},
                {"audio/aiff", ".aiff"},
                {"audio/basic", ".snd"},
                {"audio/mid", ".midi"},
                {"audio/mp4", ".m4a"}, // one way mapping only, mime -> ext
                {"audio/wav", ".wav"},
                {"audio/x-m4a", ".m4a"},
                {"audio/x-mpegurl", ".m3u"},
                {"audio/x-pn-realaudio", ".ra"},
                {"audio/x-smd", ".smd"},
                {"image/bmp", ".bmp"},
                {"image/jpeg", ".jpg"},
                {"image/pict", ".pic"},
                {"image/png", ".png"}, // Defined in [RFC-2045], [RFC-2048]
                {"image/x-png", ".png"}, // See https://www.w3.org/TR/PNG/#A-Media-type :"It is recommended that implementations also recognize the media type "image/x-png"."
                {"image/tiff", ".tiff"},
                {"image/x-macpaint", ".mac"},
                {"image/x-quicktime", ".qti"},
                {"message/rfc822", ".eml"},
                {"text/calendar", ".ics"},
                {"text/html", ".html"},
                {"text/plain", ".txt"},
                {"text/scriptlet", ".wsc"},
                {"text/xml", ".xml"},
                {"video/3gpp", ".3gp"},
                {"video/3gpp2", ".3gp2"},
                {"video/mp4", ".mp4"},
                {"video/mpeg", ".mpg"},
                {"video/quicktime", ".mov"},
                {"video/vnd.dlna.mpeg-tts", ".m2t"},
                {"video/x-dv", ".dv"},
                {"video/x-la-asf", ".lsf"},
                {"video/x-ms-asf", ".asf"},
                {"x-world/x-vrml", ".xof"},

                #endregion
            };

            var cache = mappings.ToList(); //need ToList() to avoid modifying while still enumerating

            foreach (var mapping in cache)
            {
                if (!mappings.ContainsKey(mapping.Value))
                {
                    mappings.Add(mapping.Value, mapping.Key);
                }
            }

            return mappings;
        }

        /// <summary>
        /// Tries to get the type of the MIME from the provided string.
        /// </summary>
        /// <param name="str">The filename or extension.</param>
        /// <param name="mimeType">The variable to store the MIME type.</param>
        /// <returns>Whether the transaction was completed successfully.</returns>
        /// <exception cref="ArgumentNullException" />
        public virtual bool TryGetMimeType(string str, out string mimeType)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (!str.StartsWith(Dot))
            {
                var index = str.LastIndexOf(Dot);
                if (index != -1 && str.Length > index + 1)
                {
                    str = str.Substring(index + 1);
                }

                str = Dot + str;
            }

            var indexQuestionMark = str.IndexOf(QuestionMark, StringComparison.Ordinal);
            if (indexQuestionMark != -1)
            {
                str = str.Remove(indexQuestionMark);
            }

            return MappingDictionary.Value.TryGetValue(str, out mimeType);
        }

        /// <summary>
        /// Gets the type of the MIME from the provided string.
        /// </summary>
        /// <param name="str">The filename or extension.</param>
        /// <param name="throwErrorIfNotFound">if set to <c>true</c>, throws error if extension's not found.</param>
        /// <returns>The MIME type.</returns>
        /// <exception cref="ArgumentNullException" />
        public virtual string GetMimeType(string str, bool throwErrorIfNotFound = true)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (TryGetMimeType(str, out var result))
            {
                return result;
            }

            if (throwErrorIfNotFound)
            {
                throw new ArgumentException("Requested mime type is not registered for: " + str);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the extension from the provided MIME type.
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="extension">The variable to store the extension.</param>
        /// <returns>Whether the transaction was completed successfully.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public virtual bool TryGetExtension(string mimeType, out string extension)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
            {
                throw new ArgumentNullException(nameof(mimeType));
            }

            if (mimeType.StartsWith(Dot))
            {
                throw new ArgumentException("Requested mime type is not valid: " + mimeType);
            }

            return MappingDictionary.Value.TryGetValue(mimeType, out extension);
        }

        /// <summary>
        /// Gets the extension from the provided MIME type.
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="throwErrorIfNotFound">if set to <c>true</c>, throws error if extension's not found.</param>
        /// <returns>The extension.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public virtual string GetExtension(string mimeType, bool throwErrorIfNotFound = true)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
            {
                throw new ArgumentNullException(nameof(mimeType));
            }

            if (TryGetExtension(mimeType, out string extension))
            {
                return extension;
            }

            if (throwErrorIfNotFound)
            {
                throw new ArgumentException("Requested mime type is not registered: " + mimeType);
            }

            return string.Empty;
        }

        /// <summary>
        /// Adds MIME type to map
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="extension">Type of the extension</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public void AddMimeType(string mimeType, string extension)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
            {
                throw new ArgumentNullException(nameof(mimeType));
            }

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentNullException(nameof(extension));
            }

            if (mimeType.StartsWith(Dot))
            {
                throw new ArgumentException("MIME type should not start with dot");
            }
            
            if (!extension.StartsWith(Dot))
            {
                throw new ArgumentException("Extensions should start with dot");
            }

            if (MappingDictionary.Value.ContainsKey(mimeType))
            {
                throw new ArgumentException("An item with the same mimeType has already been added");
            }

            MappingDictionary.Value.Add(mimeType, extension);
        }

        /// <summary>
        /// Removes MIME type from map
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public void RemoveMimeType(string mimeType)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
            {
                throw new ArgumentNullException(nameof(mimeType));
            }

            if (mimeType.StartsWith(Dot))
            {
                throw new ArgumentException("MIME type should not start with dot");
            }

            if (MappingDictionary.Value.ContainsKey(mimeType))
            {
                MappingDictionary.Value.Remove(mimeType);
            }
        }

        /// <summary>
        /// Adds extension to map
        /// </summary>
        /// <param name="extension">Type of the extension</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public void AddExtension(string extension, string mimeType)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentNullException(nameof(extension));
            }

            if (string.IsNullOrWhiteSpace(mimeType))
            {
                throw new ArgumentNullException(nameof(mimeType));
            }

            if (MappingDictionary.Value.ContainsKey(extension))
            {
                throw new ArgumentException("An item with the same extension has already been added");
            }

            if (mimeType.StartsWith(Dot))
            {
                throw new ArgumentException("MIME type should not start with dot");
            }
            
            if (!extension.StartsWith(Dot))
            {
                throw new ArgumentException("Extensions should start with dot");
            }

            MappingDictionary.Value.Add(extension, mimeType);
        }

        /// <summary>
        /// Removes extension from map
        /// </summary>
        /// <param name="extension">Type of the extension</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public void RemoveExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentNullException(nameof(extension));
            }

            if (!extension.StartsWith(Dot))
            {
                throw new ArgumentException("Extensions should start with dot");
            }

            if (MappingDictionary.Value.ContainsKey(extension))
            {
                MappingDictionary.Value.Remove(extension);
            }
        }
    }
}