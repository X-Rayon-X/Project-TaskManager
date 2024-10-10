// Ignore Spelling: ull

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CourseWork_TaskManager
{
    public partial class Form1 : Form
    {
        private List<Process> processes = null;

        private ListComparer comparer = null;

        private float cpu;
        private float ram;
        private float installedMemory;
        public Form1()
        {
            InitializeComponent();
        }

        private void GetProcesses()
        {
            processes.Clear();

            processes = Process.GetProcesses().ToList<Process>();
        }

        private void RefreshProcessList()
        {
            listView1.Items.Clear();

            double memsize = 0;

            foreach (Process process in processes)
            {
                memsize = 0;

                PerformanceCounter pcounter = new PerformanceCounter();
                pcounter.CategoryName = "Process";
                pcounter.CounterName = "Working set - Private";
                pcounter.InstanceName = process.ProcessName;

                memsize = (double)pcounter.NextValue() / (1000 * 1000);
                string[] row = new string[] { process.ProcessName.ToString(), Math.Round(memsize, 1).ToString() };
                listView1.Items.Add(new ListViewItem(row));

                pcounter.Close();
                pcounter.Dispose();
            }

            Text = "Task Manager - Running Processes: " + processes.Count.ToString();
        }

        private void RefreshProcessList(List<Process> processes, string keyword)
        {
            try
            {

                listView1.Items.Clear();

                double memsize = 0;

                foreach (Process process in processes)
                {
                    if (process != null)
                    {
                        memsize = 0;

                        PerformanceCounter pcounter = new PerformanceCounter();
                        pcounter.CategoryName = "Process";
                        pcounter.CounterName = "Working set - Private";
                        pcounter.InstanceName = process.ProcessName;

                        memsize = (double)pcounter.NextValue() / (1000 * 1000);
                        string[] row = new string[] { process.ProcessName.ToString(), Math.Round(memsize, 1).ToString() };
                        listView1.Items.Add(new ListViewItem(row));

                        pcounter.Close();
                        pcounter.Dispose();
                    }
                }

                Text = $"Task Manager - Running Processes: '{keyword}'" + processes.Count.ToString();
            }
            catch (Exception) { }
        }

        private void KillProcess(Process process)
        {
            process.Kill();

            process.WaitForExit();
        }

        private void KillProcessAndChildren(int processId)
        {
            if (processId == 0)
            {
                return;
            }

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "Select * From Win32_Process Where ParentProcessID=" + processId);

            ManagementObjectCollection objectCollection = searcher.Get();

            foreach (ManagementObject obj in objectCollection)
            {
                KillProcessAndChildren(Convert.ToInt32(obj["ProcessID"]));
            }

            try
            {
                Process process = Process.GetProcessById(processId);

                process.Kill();

                process.WaitForExit();
            }
            catch (ArgumentException) { }


        }

        private int GetParentProcessId(Process process)
        {
            int parentId = 0;

            try
            {
                ManagementObject managementObject = new ManagementObject("win32_process.handle='" + process.Id + "'");


                managementObject.Get();

                parentId = Convert.ToInt32(managementObject["ParentProcessId"]);
            }
            catch (Exception) { }

            return parentId;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            processes = new List<Process>();

            GetProcesses();
            RefreshProcessList();

            comparer = new ListComparer();
            comparer.ColumnIndex = 0;

            MEMORYSTATUSEX MEMORYSTATUSEX = new MEMORYSTATUSEX();

            if (GlobalMemoryStatusEx(MEMORYSTATUSEX))
            {
                installedMemory = MEMORYSTATUSEX.ullTotalPhys;
            }

            label10.Text = Convert.ToString(installedMemory / 1000000000) + " Gb";

            timer1.Interval = 1000;

            timer1.Start();

            timer2.Interval = 7000;
        }

        private void toolStripButton1_Click(object sender, System.EventArgs e)
        {
            if (toolStripButton1.Text == "Stop")
            {
                toolStripButton1.Text = "Update";
                timer2.Stop();
            }
            else
            {
                toolStripButton1.Text = "Stop";
                timer2.Start();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processtoKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcess(processtoKill);

                    GetProcesses();

                    RefreshProcessList();

                }
            }
            catch (Exception) { }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processtoKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcessAndChildren(GetParentProcessId(processtoKill));

                    GetProcesses();

                    RefreshProcessList();

                }
            }
            catch (Exception) { }
        }

        private void finishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processtoKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcess(processtoKill);

                    GetProcesses();

                    RefreshProcessList();

                }
            }
            catch (Exception) { }
        }

        private void completeProcessTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processtoKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcessAndChildren(GetParentProcessId(processtoKill));

                    GetProcesses();

                    RefreshProcessList();

                }
            }
            catch (Exception) { }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            GetProcesses();

            List<Process> filterprocess = processes.Where((x) =>
            x.ProcessName.ToLower().Contains(toolStripTextBox1.Text.ToLower())).ToList<Process>();

            RefreshProcessList(filterprocess, toolStripTextBox1.Text);
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            comparer.ColumnIndex = e.Column;
            comparer.SortDirections = comparer.SortDirections == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

            listView1.ListViewItemSorter = comparer;
            listView1.Sort();
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]

        private class MEMORYSTATUSEX

        {

            public uint dwLength;

            public uint dwMemoryLength;

            public ulong ullTotalPhys;

            public ulong ullAvailPhys;

            public ulong ullTotalPageFile;

            public ulong ullAvailPageFile;

            public ulong ullTotalVirtual;

            public ulong ullAvailVirtual;

            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        private void timer1_Tick(object sender, EventArgs e)
        {
            cpu = performanceCPU.NextValue();
            ram = performanceRAM.NextValue();

            progressBar1.Value = (int)cpu;

            progressBar2.Value = (int)ram;

            label2.Text = Convert.ToString(Math.Round(cpu, 1)) + " %";

            label4.Text = Convert.ToString(Math.Round(ram, 1)) + " %";

            label8.Text = Convert.ToString(Math.Round((ram / 100 * installedMemory) / 1000000000, 1)) + " Gb";

            label9.Text = Convert.ToString(Math.Round((installedMemory - ram / 100 * installedMemory) / 1000000000, 1)) + " Gb";

            chart1.Series["CPU"].Points.AddY(cpu);
            chart1.Series["RAM"].Points.AddY(ram);

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            GetProcesses();
            RefreshProcessList();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}
