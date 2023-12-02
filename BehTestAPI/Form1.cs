using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateInvDocByAPI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        protected async Task Submit()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://tous.behandish.com/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiIiLCJqdGkiOiIyYmNhMzgyNy1jZTZiLTRkMmYtYTUzYy1hMWI2YzNiNTE2M2UiLCJleHAiOjE3MDM5MTQ1NTgsImlzcyI6IldlYkJlaGFuZGlzaCIsImF1ZCI6IldlYkJlaGFuZGlzaCJ9.2DMsXHrq9s0ZJ8clvNunrinmbXgV2WZV_kbwxPd6Rk4");
                client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(new 
                {
                    #region init
                    ID = 0,
                    DocStatus = 1,
                    
                    #endregion

                    InvDocType = 1, //نوع  رویداد: رسید انبار
                    DocDate = "1402/07/18", // تاریخ
                    InvDocKind = new { ID = 20193106 }, // نوع
                    Comments = "ایجاد توسط اي پي آي",
                    QtyComments = "ایجاد توسط اي پي آي",
                    QtyStatus = 2048, //وضعیت: تایید مقداری // 1024
                    Stock = new { ID = 5611867 }, // انبار سنگان معدن
                    items = new List<object>() 
                    {
                        new 
                        {
                            ID = 0,
                            RowNo = 1,
                            Dsc = "ایجاد سطر توسط اي پي آي",
                            Good = new { Code = Convert.ToInt32(tbxGoodCode.Text) },
                            InputQty = Convert.ToInt32(tbxGoodQty.Text),
                            UnitID = (string) null,
                            SubSb = (string) null,
                        }
                    },
                });
                var invDoc = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("API/Inventory/InvDocs?InvDocType=1&IsPricedDoc=false", invDoc);

                if (response.IsSuccessStatusCode)
                {
                    var strResponse = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(strResponse);
                    MessageBox.Show($"رسيد انبار جديد با شماره: {jsonResponse.GetValue("DocNo")} و شناسه: {jsonResponse.GetValue("ID")} در انبار: سنگان معدن ايجاد شد", 
                                     "پیام", MessageBoxButtons.OK);
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"{response.StatusCode}:\n{message}", "پیام", MessageBoxButtons.OK);
                }
            }
        }

        protected void Reset()
        {
            tbxGoodCode.Text = "11001001";
            tbxGoodQty.Text = "1";
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            await Submit();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }
    }
}
