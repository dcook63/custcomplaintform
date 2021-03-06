﻿using CustServForm.CustComplaints;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace CustServForm
{
    public partial class WebComplaint : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var path = Server.MapPath(@"~/CustComplaints/xml/Locations.xml");
            XmlDocument locDoc = new XmlDocument();
            locDoc.Load(path);
            locDDList.Items.Clear();
            if (!IsPostBack)
            {
                calendar.Visible = false;

                //Load Disposition Categories into Drop Down Menu
                XmlDocument dispDoc = new XmlDocument();
                var dispPath = Server.MapPath(@"~/CustComplaints/xml/WebDispIssues.xml");
                dispDoc.Load(dispPath);
                dispList.Items.Clear();
                XmlNodeList dispNode = dispDoc.SelectNodes("/root/Unit");
                foreach (XmlElement n in dispNode)
                {
                    ListItem i = new ListItem();
                    i.Text = n.Attributes[0].Value;
                    if (!isDuplicate(dispList, i.Text)) { dispList.Items.Add(i); }
                }

                //Load disposition issues into dropdown menu
                dispDoc.Load(dispPath);
                dispNode = dispDoc.SelectNodes("/root/Unit");
                dispDetails.Items.Clear();
                foreach (XmlElement n in dispNode)
                {
                    ListItem i = new ListItem();
                    i.Text = n.Attributes[1].Value;
                    if (n.Attributes[0].Value.Equals(dispList.SelectedValue)) { dispDetails.Items.Add(i); }
                }

                //Load Origin of Complaint categories into Drop Down Menu
                XmlDocument origDoc = new XmlDocument();
                var origPath = Server.MapPath(@"~/CustComplaints/xml/OriginOfComplaint.xml");
                origDoc.Load(origPath);
                originList.Items.Clear();
                XmlNodeList origNode = origDoc.SelectNodes("/root/Unit");
                foreach (XmlElement n in origNode)
                {
                    ListItem i = new ListItem();
                    i.Text = n.Attributes[0].Value;
                    originList.Items.Add(i);
                }

                //Find xml node based on selected item of dropdown menu
                string val = originList.SelectedItem.Text;
                origDoc.Load(origPath);
                XmlNodeList origTypeNode = origDoc.SelectNodes("/root/Unit");
                foreach (XmlElement n in origTypeNode)
                {
                    if (n.Attributes[0].Value == originList.SelectedValue)
                    {
                        originTxtBox.Attributes.Add("placeholder", n.Attributes[1].Value);
                    }
                }
            }
            //Load Location XML list into Location Drop Down Menu
            XmlNodeList node = locDoc.SelectNodes("/root/Unit");
            foreach (XmlElement n in node)
            {
                ListItem i = new ListItem();
                i.Text = n.Attributes[0].Value;
                locDDList.Items.Add(i);
            }

        }

        private bool isDuplicate(DropDownList dispList, string text)
        {
            Boolean b = false;
            foreach (ListItem l in dispList.Items)
            {
                b = l.Text == text;
            }

            return b;
        }

        public void originChanged(object sender, EventArgs e)
        {
            //Find xml node based on selected item of dropdown menu
            XmlDocument origDoc = new XmlDocument();
            var path = Server.MapPath(@"~/CustComplaints/xml/OriginOfComplaint.xml");
            string val = originList.SelectedItem.Text;
            origDoc.Load(path);
            XmlNodeList origTypeNode = origDoc.SelectNodes("/root/Unit");
            foreach (XmlElement n in origTypeNode)
            {
                if (n.Attributes[0].Value == originList.SelectedValue)
                {
                    originTxtBox.Attributes.Add("placeholder", n.Attributes[1].Value);
                }
            }

        }

        public void dispListChanged(object sender, EventArgs e)
        {
            XmlDocument dispDoc = new XmlDocument();
            var dispPath = Server.MapPath(@"~/CustComplaints/xml/WebDispIssues.xml");
            //Load disposition issues into dropdown menu
            dispDoc.Load(dispPath);
            XmlNodeList dispNode = dispDoc.SelectNodes("/root/Unit");
            dispDetails.Items.Clear();
            foreach (XmlElement n in dispNode)
            {
                ListItem i = new ListItem();
                i.Text = n.Attributes[1].Value;
                if (n.Attributes[0].Value.Equals(dispList.SelectedValue)) { dispDetails.Items.Add(i); }
            }
        }

        public void dateChanged(object sender, EventArgs e)
        {
            dateTextBox.Text = (calendar.SelectedDate.ToShortDateString());
            calendar.Visible = false;
        }

        public void fp_selectedIndexChanged(object sender, EventArgs e)
        {
            if (FP_Radio.SelectedItem.Value == "1")
            {
                FPIDTxtBox.Visible = true;
                FPTier.Visible = true;
            }
            else
            {
                FPIDTxtBox.Visible = false;
                FPTier.Visible = false;
            }
        }

        public void showCal(object sender, EventArgs e)
        {
            if (calendar.Visible.Equals(false))
            {
                calendar.Visible = true;
            }
            else { calendar.Visible = false; }
        }

        public void SubmitForm(object sender, EventArgs e)
        {
            FormData formData = new FormData();
            formData.Fill(locDDList.SelectedItem.Text, Convert.ToInt32(FP_Radio.SelectedValue), CustEmail.Text, dateTextBox.Text, originList.SelectedItem.Text, originTxtBox.Text, dispList.SelectedItem.Text, dispDetails.SelectedItem.Text, commentBox.Text, CustName.Text, ReservationTextBox.Text);
            JObject body = formData.FormatJSON("Web Complaint");
            if (SamanageConnectAPI.PostToSamanage(body))
                ScriptManager.RegisterStartupScript(this, this.GetType(), "MyScript", "alert('Form Submitted Successfully!')", true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "MyScript", "alert('Form Failed to Submit...')", true);
        }
    }
}