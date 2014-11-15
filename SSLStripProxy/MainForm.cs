using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SSLStripProxy.Http;

namespace SSLStripProxy
{
    public partial class MainForm : Form
    {
        private class Request
        {
            public int Id { get; set; }
            public HttpRequest HttpRequest { get; set; }
            public HttpResponse HttpResponse { get; set; }
            public string Uri { get; set; }
            public string Client { get; set; }

            public override string ToString()
            {
                return Client + " " + Uri;
            }
        }

        readonly BindingList<Request> _requestsList = new BindingList<Request>();
        readonly BindingList<string> _selectedMethods = new BindingList<string>();
        readonly BindingList<string> _selectedContentTypes = new BindingList<string>();

        Match _m;

        private Object _locker = new object();

        public MainForm()
        {
            InitializeComponent();
            requestsListBox.DataSource = _requestsList;
            methodListBox.DataSource = _selectedMethods;
            contentTypeListBox.DataSource = _selectedContentTypes;
        }

        public void LogRequest(int id, HttpRequest httpRequest)
        {
            MethodInvoker methodInvokerDelegate = delegate()
            {
                _requestsList.Add(new Request() { Id = id, HttpRequest = httpRequest, Uri = String.Empty, Client = String.Empty});
                if (_requestsList.Count > 1000) _requestsList.RemoveAt(0);
            };

            if (InvokeRequired)
                Invoke(methodInvokerDelegate);
            else
                methodInvokerDelegate();
        }

        public void LogResponse(int id, HttpResponse httpResponse)
        {
            MethodInvoker methodInvokerDelegate = delegate()
            {
                foreach (var request in _requestsList)
                {
                    if (request.Id == id)
                    {
                        request.HttpResponse = httpResponse;
                        break;
                    }
                }
            };

            if (InvokeRequired)
                Invoke(methodInvokerDelegate);
            else
                methodInvokerDelegate();
        }

        public void LogUri(int id, string uri)
        {
            MethodInvoker methodInvokerDelegate = delegate()
            {
                for (int i = 0; i < _requestsList.Count; i++)
                {
                    var request = _requestsList[i];
                    if (request.Id == id)
                    {
                        request.Uri = uri;
                        _requestsList.ResetItem(i);
                    }
                }
            };

            if (InvokeRequired)
                Invoke(methodInvokerDelegate);
            else
                methodInvokerDelegate();
        }

        public void LogClient(int id, string client)
        {
            MethodInvoker methodInvokerDelegate = delegate()
            {
                for (int i = 0; i < _requestsList.Count; i++)
                {
                    var request = _requestsList[i];
                    if (request.Id == id)
                    {
                        request.Client = client;
                        _requestsList.ResetItem(i);
                    }
                }
            };

            if (InvokeRequired)
                Invoke(methodInvokerDelegate);
            else
                methodInvokerDelegate();
        }

       
        private void addMethodButton_Click(object sender, EventArgs e)
        {
            string method = methodComboBox.Text;
            if (!String.IsNullOrEmpty(method) && !_selectedMethods.Contains(method))
                _selectedMethods.Add(method);
        }

        private void removeMethodButton_Click(object sender, EventArgs e)
        {
            string method = methodComboBox.Text;
            if (!String.IsNullOrEmpty(method))
                _selectedMethods.Remove(method);
        }

        private void methodsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string method = (string)methodListBox.SelectedItem;
            if (!String.IsNullOrEmpty(method))
                methodComboBox.Text = method;

        }

        private void addContentTypeButton_Click(object sender, EventArgs e)
        {
            string contentType = contentTypeComboBox.Text;
            if (!String.IsNullOrEmpty(contentType) && !_selectedContentTypes.Contains(contentType))
                _selectedContentTypes.Add(contentType);
        }

        private void removeContentTypeButton_Click(object sender, EventArgs e)
        {
            string contentType = contentTypeComboBox.Text;
            if (!String.IsNullOrEmpty(contentType))
                _selectedContentTypes.Remove(contentType);
        }

        private void setFilterButton_Click(object sender, EventArgs e)
        {
            Logger.Filter = new Logger.LoggingFilter
            { 
                Host = hostTextBox.Text.ToLower(), 
                Methods = new HashSet<string>(_selectedMethods.Select(x=>x.ToLower())), 
                ContentTypes = new HashSet<string>(_selectedContentTypes.Select(x=>x.ToLower())) 
            };
        }

        private void initMatchingButton_Click_1(object sender, EventArgs e)
        {
            Request selectedRequest = (Request)requestsListBox.SelectedItem;
            if (selectedRequest == null) return;

            string pattern = regexTextBox.Text;
            var content = Encoding.UTF8.GetString(selectedRequest.HttpRequest.Compile()) + Encoding.UTF8.GetString(selectedRequest.HttpResponse.Compile());
            try
            {
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                _m = r.Match(content);
            }
            catch (Exception exception)
            {
                
            }
            
        }

        private void nextMatchButton_Click_1(object sender, EventArgs e)
        {
            if (_m == null)
                return;

            int groupNr;

            if (String.IsNullOrEmpty(groupNrTextBox.Text))
            {
                groupNr = 0;
            }
            else
            {
                try
                {
                    groupNr = int.Parse(groupNrTextBox.Text);
                }
                catch (Exception exception)
                {
                    return;
                }
            }

            if (_m.Success)
            {
                Group group = _m.Groups[groupNr];

                MethodInvoker methodInvokerDelegate = delegate()
                {
                    matchTextBox.Text = group.Value;
                };

                if (InvokeRequired)
                    Invoke(methodInvokerDelegate);
                else
                    methodInvokerDelegate();

                _m = _m.NextMatch();
            }
            else
            {
                MethodInvoker methodInvokerDelegate = delegate()
                {
                    matchTextBox.Text = String.Empty;
                };

                if (InvokeRequired)
                    Invoke(methodInvokerDelegate);
                else
                    methodInvokerDelegate();
            }
        }
    }
}
