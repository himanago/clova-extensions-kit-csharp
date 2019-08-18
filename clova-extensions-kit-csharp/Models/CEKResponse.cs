﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace LineDC.CEK.Models
{
    /// <summary>
    /// CEK esponse Message
    /// https://clova-developers.line.me/guide/#/CEK/References/CEK_API.md#CustomExtResponseMessage
    /// </summary>
    public class CEKResponse
    {
        /// <summary>
        /// Extensionのレスポンス情報を含むオブジェクト
        /// </summary>
        [JsonProperty("response")]
        public Response Response { get; set; }
        /// <summary>
        /// ユーザーとのマルチターン対話に必要な情報を保存するために使用されるオブジェクト。Custom Extensionは、sessionAttributesフィールドを使用して途中までの情報をCEKに渡します。ユーザーの追加のリクエストを受け付けると、その情報は再びリクエストメッセージのsession.sessionAttributesフィールドで渡されます。sessionAttributesオブジェクトは、キー(key)と値(value)のペアで構成され、Custom Extensionを実装する際に任意で定義できます。保存する値がない場合、空のオブジェクトを入力します。
        /// </summary>
        [JsonProperty("sessionAttributes")]
        public Dictionary<string, object> SessionAttributes { get; set; }
        /// <summary>
        /// メッセージフォーマットのバージョン(CEKのバージョン)
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonIgnore]
        public bool ShouldEndSession
        {
            get
            {
                return Response.ShouldEndSession;
            }
            private set
            {
                Response.ShouldEndSession = value;
            }
        }

        /// <summary>
        /// Default response language
        /// </summary>
        [JsonIgnore]
        public Lang DefaultLang { get; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="version">CEK応答バージョン</param>
        public CEKResponse(string version = "1.0", Lang defaultLang = Lang.Ja)
        {
            Response = new Response()
            {
                Directives = new List<Directive>(),
                OutputSpeech = new OutputSpeech()
                {
                    Type = OutputSpeechType.SimpleSpeech,
                    Values = new List<SpeechInfoObject>()
                },
                ShouldEndSession = true,
                Reprompt = new Reprompt()
                {
                    OutputSpeech = new OutputSpeech()
                    {
                        Type = OutputSpeechType.SimpleSpeech,
                        Values = new List<SpeechInfoObject>()
                    },
                },
                Card = new object()
            };
            Version = version;
            SessionAttributes = new Dictionary<string, object>();
            DefaultLang = defaultLang;
        }

        /// <summary>
        /// 音声情報を返す - テキスト
        /// </summary>
        /// <param name="text">音声用テキスト</param>
        /// <param name="lang">言語</param>
        public CEKResponse AddText(string text, Lang? lang = null)
        {
            if (Response.OutputSpeech.Values.Count > 0)
                Response.OutputSpeech.Type = OutputSpeechType.SpeechList;

            Response.OutputSpeech.Values.Add(new SpeechInfoObject()
            {
                Type = SpeechInfoType.PlainText,
                Lang = lang ?? DefaultLang,
                Value = text
            });
            return this;
        }

        public void KeepListen()
        {
            Response.ShouldEndSession = false;
        }

        /// <summary>
        /// 音声情報を返す - URL
        /// </summary>
        /// <param name="url">音声ファイルのパス</param>
        public CEKResponse AddUrl(string url)
        {
            if (Response.OutputSpeech.Values.Count > 0)
                Response.OutputSpeech.Type = OutputSpeechType.SpeechList;
            Response.OutputSpeech.Values.Add(new SpeechInfoObject()
            {
                Type = SpeechInfoType.URL,
                Lang = null,
                Value = url
            });
            return this;
        }

        /// <summary>
        /// マルチターン対話で、対話の中間情報を保存する
        /// </summary>
        /// <param name="key">セッション情報のキー</param>
        /// <param name="value">セッション情報の値</param>
        public CEKResponse SetSession(string key, object value)
        {
            if (SessionAttributes.ContainsKey(key))
                SessionAttributes[key] = value;
            else
                SessionAttributes.Add(key, value);
            return this;
        }

        /// <summary>
        /// マルチターン対話でユーザーの追加の発話を促す - テキスト
        /// </summary>
        /// <param name="text">音声用テキスト</param>
        /// <param name="lang">言語</param>
        public CEKResponse AddRepromptText(string text, Lang? lang = null)
        {
            if (Response.Reprompt.OutputSpeech.Values.Count > 0)
                Response.Reprompt.OutputSpeech.Type = OutputSpeechType.SpeechList;

            Response.Reprompt.OutputSpeech.Values.Add(new SpeechInfoObject()
            {
                Type = SpeechInfoType.PlainText,
                Lang = lang ?? DefaultLang,
                Value = text
            });
            return this;
        }

        /// <summary>
        /// マルチターン対話でユーザーの追加の発話を促す - URL
        /// </summary>
        /// <param name="url">音声ファイルのパス</param>
        /// <param name="lang">言語</param>
        public CEKResponse AddRepromptUrl(string url)
        {
            if (Response.Reprompt.OutputSpeech.Values.Count > 0)
                Response.Reprompt.OutputSpeech.Type = OutputSpeechType.SpeechList;

            Response.Reprompt.OutputSpeech.Values.Add(new SpeechInfoObject()
            {
                Type = SpeechInfoType.URL,
                Lang = null,
                Value = url
            });
            return this;
        }

        /// <summary>
        /// 複合タイプ(SpeechSet)の音声情報を返す - 要約 テキスト
        /// </summary>
        /// <param name="text">音声用テキスト</param>
        /// <param name="lang">言語</param>
        public CEKResponse AddBriefText(string text, Lang? lang = null)
        {
            Response.OutputSpeech.Type = OutputSpeechType.SpeechSet;
            Response.OutputSpeech.Values = null;
            Response.OutputSpeech.Verbose = new Verbose()
            {
                Type = OutputSpeechType.SimpleSpeech,
                Values = new List<SpeechInfoObject>()
            };
            Response.OutputSpeech.Brief = new SpeechInfoObject()
            {
                Type = SpeechInfoType.PlainText,
                Lang = lang ?? DefaultLang,
                Value = text
            };
            return this;
        }

        /// <summary>
        /// 複合タイプ(SpeechSet)の音声情報を返す - 要約 URL
        /// </summary>
        /// <param name="url">音声ファイルのパス</param>
        /// <param name="lang">言語</param>
        public CEKResponse AddBriefUrl(string url)
        {
            Response.OutputSpeech.Type = OutputSpeechType.SpeechSet;
            Response.OutputSpeech.Values = null;
            Response.OutputSpeech.Verbose = new Verbose()
            {
                Type = OutputSpeechType.SimpleSpeech,
                Values = new List<SpeechInfoObject>()
            };
            Response.OutputSpeech.Brief = new SpeechInfoObject()
            {
                Type = SpeechInfoType.URL,
                Lang = null,
                Value = url
            };
            return this;
        }

        /// <summary>
        /// 複合タイプ(SpeechSet)の音声情報を返す - 詳細 テキスト
        /// </summary>
        /// <param name="text">音声用テキスト</param>
        /// <param name="lang">言語</param>
        public CEKResponse AddVerboseText(string text, Lang? lang = null)
        {
            if (Response.OutputSpeech.Verbose.Values.Count > 0)
                Response.OutputSpeech.Verbose.Type = OutputSpeechType.SpeechList;

            Response.OutputSpeech.Verbose.Values.Add(new SpeechInfoObject()
            {
                Type = SpeechInfoType.PlainText,
                Lang = lang ?? DefaultLang,
                Value = text
            });
            return this;
        }

        /// <summary>
        /// 複合タイプ(SpeechSet)の音声情報を返す - 詳細 URL
        /// </summary>
        /// <param name="url">音声ファイルのパス</param>
        /// <param name="lang">言語</param>
        public CEKResponse AddVerboseUrl(string url)
        {
            if (Response.OutputSpeech.Verbose.Values.Count > 0)
                Response.OutputSpeech.Verbose.Type = OutputSpeechType.SpeechList;

            Response.OutputSpeech.Verbose.Values.Add(new SpeechInfoObject()
            {
                Type = SpeechInfoType.URL,
                Lang = null,
                Value = url
            });
            return this;
        }

        /// <summary>
        /// クラスの情報を SessionAttributes に設定する。
        /// </summary>
        /// <param name="sessionValue">セッション情報</param>
        public CEKResponse SetSessionAttributesFrom(object sessionValue)
        {
            SessionAttributes.Clear();
            if (sessionValue != null)
            {
                JsonConvert.PopulateObject(JsonConvert.SerializeObject(sessionValue), SessionAttributes);
            }
            return this;
        }
    }
}
