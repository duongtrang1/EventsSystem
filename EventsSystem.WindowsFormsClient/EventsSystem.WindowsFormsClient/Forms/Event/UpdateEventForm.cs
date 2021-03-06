﻿namespace EventsSystem.WindowsFormsClient.Forms.Event
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Windows.Forms;

    public partial class UpdateEventForm : Form
    {
        private Uri URI_PUT_EVENT;
        private Uri URI_GET_CATEGORIES;
        private Uri URI_GET_TOWNS;
        private MainForm parent;

        public UpdateEventForm()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.UpdateAnEvent();
        }

        private async void UpdateAnEvent()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string startDate = this.startDateTimePicker.Value.ToString("yyyy-MM-ddThh:mm:ss.0");
                    string endDate = this.endDateTimePicker.Value.ToString("yyyy-MM-ddThh:mm:ss.0");
                    var raw = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Name", this.nameTextBox.Text.ToString()),
                        new KeyValuePair<string, string>("ShortDescrtiption", this.shortDescriptionTextBox.Text.ToString()),
                        new KeyValuePair<string, string>("IsPrivate", this.isPrivateCheckBox.Checked.ToString()),
                        new KeyValuePair<string, string>("StartDate", startDate),
                        new KeyValuePair<string, string>("EndDate", endDate),
                        new KeyValuePair<string, string>("Town", (string)this.comboBoxTowns.SelectedItem),
                        new KeyValuePair<string, string>("Category", (string)this.comboBoxCategory.SelectedItem),
                        new KeyValuePair<string, string>("CommentsCount", this.commentsNumeric.Value.ToString())
                    };

                    var content = new FormUrlEncodedContent(raw);
                    //
                    var link = string.Format(this.URI_PUT_EVENT.ToString(), this.numericUpDownId.Value.ToString());
                    using (var response = await client.PutAsync(link, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("The event was updated.");
                        }
                        else
                        {
                            MessageBox.Show(response.ReasonPhrase, "Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void GetCategories()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(this.URI_GET_CATEGORIES))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var pulledCategories = await response.Content.ReadAsStringAsync();
                            //var stuff = JsonConvert.DeserializeObject< CategoryStruct>(pulledCategories);
                            dynamic dynamicCategories = JsonConvert.DeserializeObject(pulledCategories);
                            foreach (var cat in dynamicCategories)
                            {
                                string catName = (string)cat.Name;
                                this.comboBoxCategory.Items.Add(catName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could\'t pull and populate data!", ex.Message);
            }
        }

        private async void GetTowns()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(this.URI_GET_TOWNS))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var pulledTowns = await response.Content.ReadAsStringAsync();
                            dynamic dynamicTowns = JsonConvert.DeserializeObject(pulledTowns);
                            foreach (var cat in dynamicTowns)
                            {
                                string catName = (string)cat.Name;
                                this.comboBoxTowns.Items.Add(catName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could\'t pull and populate data!", ex.Message);
            }
        }

        private void UpdateEventForm_Load(object sender, EventArgs e)
        {
            this.parent = (MainForm)this.MdiParent;
            this.URI_PUT_EVENT = new Uri(this.parent.BaseLink + "api/events/{0}");
            this.URI_GET_CATEGORIES = new Uri(this.parent.BaseLink + "api/categories");
            this.URI_GET_TOWNS = new Uri(this.parent.BaseLink + "api/towns");
            this.GetTowns();
            this.GetCategories();
            this.startDateTimePicker.MinDate = DateTime.Today;
            this.endDateTimePicker.MinDate = this.startDateTimePicker.MinDate.AddDays(1).AddHours(1);
        }
    }
}
