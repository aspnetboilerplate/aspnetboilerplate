//using DotNetOpenAuth.OAuth.ChannelElements;
//using DotNetOpenAuth.OAuth2;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Web;
//using Yishe.Abp.Core.Security;

//namespace Yishe.Abp.Handler.OAuth
//{
//    public class OAuthTokenHandler : DelegatingHandler
//    {
//        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var headers = request.Headers;
//                if (headers.Authorization != null)
//                {
//                    if (headers.Authorization.Scheme.Equals("Bearer"))
//                    {
//                        string accessToken = request.Headers.Authorization.Parameter;

//                        ResourceServer server = new ResourceServer(
//                                        new StandardAccessTokenAnalyzer(
//                                            (RSACryptoServiceProvider)OAuthCertificateHelper.SigningCertificate.PublicKey.Key,
//                                            (RSACryptoServiceProvider)OAuthCertificateHelper.EncryptionCertificate.PrivateKey
//                                        )
//                                    );

//                        OAuthPrincipal principal = server.GetPrincipal() as OAuthPrincipal;
//                        if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
//                        {
//                            var claims = new List<Claim>();

//                            foreach (string scope in principal.Roles)
//                                claims.Add(new Claim("http://fugao/OAuth20/claims/scope", scope));

//                            claims.Add(new Claim(ClaimTypes.Name, principal.Identity.Name));
//                            //var openValid = System.Configuration.ConfigurationManager.AppSettings["ValidValue"];
//                            //if (openValid.Equals("1"))
//                            //{
//                            //    ///判断用户是否合法
//                            //    var autoValid = AutoValidModel.GetInstence();
//                            //    if (string.IsNullOrEmpty(accessToken) || (autoValid.ExitToken(int.Parse(principal.Identity.Name)) && !autoValid.EqualsToken(int.Parse(principal.Identity.Name), accessToken)))
//                            //    {
//                            //        //1.用户不属于合法memberId时2.用的token与合法的不一致时
//                            //        var response2 = request.CreateResponse(HttpStatusCode.Unauthorized);

//                            //        response2.Headers.WwwAuthenticate.Add(
//                            //                new AuthenticationHeaderValue("Bearer", "error=\"invalid_token\""));

//                            //        return response2;

//                            //    }
//                            //}
//                            var identity = new ClaimsIdentity(claims, "Bearer");
//                            var newPrincipal = new ClaimsPrincipal(identity);

//                            //设置User对象到上下文
//                            Thread.CurrentPrincipal = newPrincipal;

//                            if (HttpContext.Current != null)
//                                HttpContext.Current.User = newPrincipal;
//                        }
//                    }
//                }

//                var response = await base.SendAsync(request, cancellationToken);

//                if (response.StatusCode == HttpStatusCode.Unauthorized)
//                {
//                    response.Headers.WwwAuthenticate.Add(
//                                new AuthenticationHeaderValue("Bearer",
//                                    "error=\"invalid_token\""));
//                }

//                return response;
//            }
//            catch (Exception)
//            {
//                var response = request.CreateResponse(HttpStatusCode.Unauthorized);

//                response.Headers.WwwAuthenticate.Add(
//                        new AuthenticationHeaderValue("Bearer", "error=\"invalid_token\""));

//                return response;
//            }
//        }
//    }
//}
