using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace PacketCreator
{
    public partial class MainForm : Form
    {
        private byte[] packet;
        
        public MainForm()
        {
            InitializeComponent();
            InitializeProtocolComboBox();
        }
        
        private void InitializeProtocolComboBox()
        {
            cmbProtocol.Items.AddRange(new object[] { "Ethernet", "IPv4", "IPv6", "TCP", "UDP", "ICMP", "Raw" });
            cmbProtocol.SelectedIndex = 0;
        }
        
        private void InitializeComponent()
        {
            this.cmbProtocol = new System.Windows.Forms.ComboBox();
            this.pnlFields = new System.Windows.Forms.Panel();
            this.btnCreatePacket = new System.Windows.Forms.Button();
            this.txtHexPreview = new System.Windows.Forms.TextBox();
            this.btnSendPacket = new System.Windows.Forms.Button();
            this.lblProtocol = new System.Windows.Forms.Label();
            this.lblHexPreview = new System.Windows.Forms.Label();
            this.SuspendLayout();
            
            // lblProtocol
            this.lblProtocol.AutoSize = true;
            this.lblProtocol.Location = new System.Drawing.Point(12, 15);
            this.lblProtocol.Name = "lblProtocol";
            this.lblProtocol.Size = new System.Drawing.Size(55, 13);
            this.lblProtocol.Text = "Protocol:";
            
            // cmbProtocol
            this.cmbProtocol.FormattingEnabled = true;
            this.cmbProtocol.Location = new System.Drawing.Point(73, 12);
            this.cmbProtocol.Name = "cmbProtocol";
            this.cmbProtocol.Size = new System.Drawing.Size(121, 21);
            this.cmbProtocol.TabIndex = 0;
            this.cmbProtocol.SelectedIndexChanged += new System.EventHandler(this.cmbProtocol_SelectedIndexChanged);
            
            // pnlFields
            this.pnlFields.AutoScroll = true;
            this.pnlFields.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFields.Location = new System.Drawing.Point(12, 39);
            this.pnlFields.Name = "pnlFields";
            this.pnlFields.Size = new System.Drawing.Size(560, 250);
            this.pnlFields.TabIndex = 1;
            
            // lblHexPreview
            this.lblHexPreview.AutoSize = true;
            this.lblHexPreview.Location = new System.Drawing.Point(12, 300);
            this.lblHexPreview.Name = "lblHexPreview";
            this.lblHexPreview.Size = new System.Drawing.Size(72, 13);
            this.lblHexPreview.Text = "Hex Preview:";
            
            // txtHexPreview
            this.txtHexPreview.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHexPreview.Location = new System.Drawing.Point(12, 316);
            this.txtHexPreview.Multiline = true;
            this.txtHexPreview.Name = "txtHexPreview";
            this.txtHexPreview.ReadOnly = true;
            this.txtHexPreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtHexPreview.Size = new System.Drawing.Size(560, 100);
            this.txtHexPreview.TabIndex = 2;
            
            // btnCreatePacket
            this.btnCreatePacket.Location = new System.Drawing.Point(12, 422);
            this.btnCreatePacket.Name = "btnCreatePacket";
            this.btnCreatePacket.Size = new System.Drawing.Size(100, 30);
            this.btnCreatePacket.TabIndex = 3;
            this.btnCreatePacket.Text = "Create Packet";
            this.btnCreatePacket.UseVisualStyleBackColor = true;
            this.btnCreatePacket.Click += new System.EventHandler(this.btnCreatePacket_Click);
            
            // btnSendPacket
            this.btnSendPacket.Location = new System.Drawing.Point(118, 422);
            this.btnSendPacket.Name = "btnSendPacket";
            this.btnSendPacket.Size = new System.Drawing.Size(100, 30);
            this.btnSendPacket.TabIndex = 4;
            this.btnSendPacket.Text = "Send Packet";
            this.btnSendPacket.UseVisualStyleBackColor = true;
            this.btnSendPacket.Click += new System.EventHandler(this.btnSendPacket_Click);
            
            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.btnSendPacket);
            this.Controls.Add(this.btnCreatePacket);
            this.Controls.Add(this.txtHexPreview);
            this.Controls.Add(this.lblHexPreview);
            this.Controls.Add(this.pnlFields);
            this.Controls.Add(this.cmbProtocol);
            this.Controls.Add(this.lblProtocol);
            this.Name = "MainForm";
            this.Text = "Custom Packet Creator";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        private System.Windows.Forms.ComboBox cmbProtocol;
        private System.Windows.Forms.Panel pnlFields;
        private System.Windows.Forms.Button btnCreatePacket;
        private System.Windows.Forms.TextBox txtHexPreview;
        private System.Windows.Forms.Button btnSendPacket;
        private System.Windows.Forms.Label lblProtocol;
        private System.Windows.Forms.Label lblHexPreview;
        
        private Dictionary<string, List<FieldControl>> protocolFields = new Dictionary<string, List<FieldControl>>();
        
        private void cmbProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlFields.Controls.Clear();
            string protocol = cmbProtocol.SelectedItem.ToString();
            
            if (!protocolFields.ContainsKey(protocol))
            {
                List<FieldControl> fields = new List<FieldControl>();
                
                switch (protocol)
                {
                    case "Ethernet":
                        fields.Add(new FieldControl("Destination MAC", "FF:FF:FF:FF:FF:FF", 6, FieldType.MAC));
                        fields.Add(new FieldControl("Source MAC", "00:11:22:33:44:55", 6, FieldType.MAC));
                        fields.Add(new FieldControl("EtherType", "0800", 2, FieldType.HEX));
                        break;
                    case "IPv4":
                        fields.Add(new FieldControl("Version", "4", 1, FieldType.NIBBLE));
                        fields.Add(new FieldControl("IHL", "5", 1, FieldType.NIBBLE));
                        fields.Add(new FieldControl("DSCP", "00", 1, FieldType.BYTE));
                        fields.Add(new FieldControl("Total Length", "0028", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Identification", "0000", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Flags", "0", 1, FieldType.NIBBLE));
                        fields.Add(new FieldControl("Fragment Offset", "0000", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("TTL", "40", 1, FieldType.BYTE));
                        fields.Add(new FieldControl("Protocol", "06", 1, FieldType.BYTE));
                        fields.Add(new FieldControl("Header Checksum", "0000", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Source IP", "192.168.1.1", 4, FieldType.IP));
                        fields.Add(new FieldControl("Destination IP", "192.168.1.2", 4, FieldType.IP));
                        break;
                    case "IPv6":
                        fields.Add(new FieldControl("Version", "6", 1, FieldType.NIBBLE));
                        fields.Add(new FieldControl("Traffic Class", "00", 1, FieldType.BYTE));
                        fields.Add(new FieldControl("Flow Label", "00000", 3, FieldType.HEX));
                        fields.Add(new FieldControl("Payload Length", "0014", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Next Header", "06", 1, FieldType.BYTE));
                        fields.Add(new FieldControl("Hop Limit", "40", 1, FieldType.BYTE));
                        fields.Add(new FieldControl("Source IP", "2001:0db8:85a3:0000:0000:8a2e:0370:7334", 16, FieldType.IPV6));
                        fields.Add(new FieldControl("Destination IP", "2001:0db8:85a3:0000:0000:8a2e:0370:7335", 16, FieldType.IPV6));
                        break;
                    case "TCP":
                        fields.Add(new FieldControl("Source Port", "1234", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Destination Port", "80", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Sequence Number", "00000000", 4, FieldType.INT));
                        fields.Add(new FieldControl("Acknowledgment Number", "00000000", 4, FieldType.INT));
                        fields.Add(new FieldControl("Data Offset", "5", 1, FieldType.NIBBLE));
                        fields.Add(new FieldControl("Reserved", "000", 1, FieldType.BITS));
                        fields.Add(new FieldControl("Flags", "002", 1, FieldType.BYTE));
                        fields.Add(new FieldControl("Window Size", "7FFF", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Checksum", "0000", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Urgent Pointer", "0000", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Data", "Hello", 0, FieldType.DATA));
                        break;
                    case "UDP":
                        fields.Add(new FieldControl("Source Port", "1234", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Destination Port", "53", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Length", "0009", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Checksum", "0000", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Data", "Hello", 0, FieldType.DATA));
                        break;
                    case "ICMP":
                        fields.Add(new FieldControl("Type", "08", 1, FieldType.BYTE));
                        fields.Add(new FieldControl("Code", "00", 1, FieldType.BYTE));
                        fields.Add(new FieldControl("Checksum", "0000", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Identifier", "0001", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Sequence Number", "0001", 2, FieldType.SHORT));
                        fields.Add(new FieldControl("Data", "abcdefgh", 0, FieldType.DATA));
                        break;
                    case "Raw":
                        fields.Add(new FieldControl("Raw Data", "", 0, FieldType.HEX));
                        break;
                }
                
                protocolFields[protocol] = fields;
            }
            
            int yPos = 10;
            foreach (var field in protocolFields[protocol])
            {
                field.Location = new Point(10, yPos);
                pnlFields.Controls.Add(field);
                yPos += field.Height + 5;
            }
        }
        
        private void btnCreatePacket_Click(object sender, EventArgs e)
        {
            try
            {
                string protocol = cmbProtocol.SelectedItem.ToString();
                List<FieldControl> fields = protocolFields[protocol];
                
                using (MemoryStream ms = new MemoryStream())
                {
                    foreach (var field in fields)
                    {
                        byte[] fieldBytes = field.GetBytes();
                        ms.Write(fieldBytes, 0, fieldBytes.Length);
                    }
                    
                    packet = ms.ToArray();
                }
                
                UpdateHexPreview();
                MessageBox.Show("Packet created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating packet: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void UpdateHexPreview()
        {
            if (packet == null || packet.Length == 0)
            {
                txtHexPreview.Text = "No packet data available.";
                return;
            }
            
            StringBuilder hexBuilder = new StringBuilder();
            StringBuilder asciiBuilder = new StringBuilder();
            
            for (int i = 0; i < packet.Length; i++)
            {
                if (i % 16 == 0)
                {
                    if (i > 0)
                    {
                        hexBuilder.Append("  ");
                        hexBuilder.Append(asciiBuilder.ToString());
                        hexBuilder.AppendLine();
                        asciiBuilder.Clear();
                    }
                    hexBuilder.AppendFormat("{0:X4}: ", i);
                }
                
                hexBuilder.AppendFormat("{0:X2} ", packet[i]);
                
                if (packet[i] >= 32 && packet[i] <= 126)
                    asciiBuilder.Append((char)packet[i]);
                else
                    asciiBuilder.Append('.');
            }
            
            int remaining = 16 - (packet.Length % 16);
            if (remaining < 16)
            {
                for (int i = 0; i < remaining; i++)
                {
                    hexBuilder.Append("   ");
                }
            }
            
            hexBuilder.Append("  ");
            hexBuilder.Append(asciiBuilder.ToString());
            
            txtHexPreview.Text = hexBuilder.ToString();
        }
        
        private void btnSendPacket_Click(object sender, EventArgs e)
        {
            if (packet == null || packet.Length == 0)
            {
                MessageBox.Show("Please create a packet first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                string protocol = cmbProtocol.SelectedItem.ToString();
                
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP))
                {
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                    
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
                    socket.Connect(endpoint);
                    
                    socket.Send(packet);
                }
                
                MessageBox.Show("Packet sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending packet: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    public enum FieldType
    {
        BYTE,
        SHORT,
        INT,
        LONG,
        MAC,
        IP,
        IPV6,
        HEX,
        DATA,
        NIBBLE,
        BITS
    }
    
    public class FieldControl : Panel
    {
        private Label lblName;
        private TextBox txtValue;
        private FieldType fieldType;
        private int byteLength;
        
        public FieldControl(string name, string defaultValue, int byteLength, FieldType fieldType)
        {
            this.fieldType = fieldType;
            this.byteLength = byteLength;
            
            this.Size = new Size(540, 30);
            
            lblName = new Label();
            lblName.AutoSize = true;
            lblName.Location = new Point(0, 8);
            lblName.Text = name + ":";
            lblName.Width = 130;
            
            txtValue = new TextBox();
            txtValue.Location = new Point(140, 5);
            txtValue.Size = new Size(400, 20);
            txtValue.Text = defaultValue;
            
            this.Controls.Add(lblName);
            this.Controls.Add(txtValue);
        }
        
        public byte[] GetBytes()
        {
            string value = txtValue.Text.Trim();
            
            switch (fieldType)
            {
                case FieldType.BYTE:
                    return new byte[] { Convert.ToByte(value, 16) };
                    
                case FieldType.SHORT:
                    ushort shortVal = Convert.ToUInt16(value, 16);
                    return new byte[] { (byte)(shortVal >> 8), (byte)(shortVal & 0xFF) };
                    
                case FieldType.INT:
                    uint intVal = Convert.ToUInt32(value, 16);
                    return new byte[] { 
                        (byte)(intVal >> 24), 
                        (byte)((intVal >> 16) & 0xFF), 
                        (byte)((intVal >> 8) & 0xFF), 
                        (byte)(intVal & 0xFF) 
                    };
                    
                case FieldType.LONG:
                    ulong longVal = Convert.ToUInt64(value, 16);
                    return new byte[] { 
                        (byte)(longVal >> 56), 
                        (byte)((longVal >> 48) & 0xFF), 
                        (byte)((longVal >> 40) & 0xFF), 
                        (byte)((longVal >> 32) & 0xFF),
                        (byte)((longVal >> 24) & 0xFF), 
                        (byte)((longVal >> 16) & 0xFF), 
                        (byte)((longVal >> 8) & 0xFF), 
                        (byte)(longVal & 0xFF) 
                    };
                    
                case FieldType.MAC:
                    string[] macParts = value.Split(':');
                    byte[] macBytes = new byte[6];
                    for (int i = 0; i < 6; i++)
                    {
                        macBytes[i] = Convert.ToByte(macParts[i], 16);
                    }
                    return macBytes;
                    
                case FieldType.IP:
                    return IPAddress.Parse(value).GetAddressBytes();
                    
                case FieldType.IPV6:
                    return IPAddress.Parse(value).GetAddressBytes();
                    
                case FieldType.HEX:
                    value = value.Replace(" ", "");
                    byte[] hexBytes = new byte[value.Length / 2];
                    for (int i = 0; i < hexBytes.Length; i++)
                    {
                        hexBytes[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
                    }
                    return hexBytes;
                    
                case FieldType.DATA:
                    return Encoding.ASCII.GetBytes(value);
                    
                case FieldType.NIBBLE:
                    byte nibble = Convert.ToByte(value, 16);
                    return new byte[] { nibble };
                    
                case FieldType.BITS:
                    byte bits = Convert.ToByte(value, 2);
                    return new byte[] { bits };
                    
                default:
                    return new byte[0];
            }
        }
    }
    
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
