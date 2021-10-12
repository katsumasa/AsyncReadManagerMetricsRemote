#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;
using Unity.IO.LowLevel.Unsafe;
using UTJ.RemoteConnect.Editor;
using UTJ.RemoteConnect;



namespace UTJ
{
    namespace AsyncReadManagerMetricsRemote
    {
        namespace Editor
        {
            public class AsyncReadManagerMetricsRemoteEditorWindow : RemoteConnectEditorWindow
            {


                private static class Styles
                {
                    public static GUIContent AverageBandwidthMBPerSecond = new GUIContent("AverageBandwidthMBPerSecond", "The mean rate of reading of data(bandwidth), in Mbps, for read request metrics included in the summary calculation.");
                    public static GUIContent AverageThroughputMBPerSeconds = new GUIContent("AverageThroughputMBPerSeconds ", "The mean rate of request throughput, in Mbps, for read request metrics included in the summary calculation.");

                    public static GUIContent AverageReadSizeInBytes = new GUIContent("AverageReadSizeInBytes", "The mean size of data read, in bytes, for read request metrics included in the summary calculation.");

                    public static GUIContent AverageReadTimeMicroseconds = new GUIContent("AverageReadTimeMicroseconds", "The mean time taken for reading(excluding queue time), in microseconds, for read request metrics included in the summary calculation.");
                    public static GUIContent AverageTotalRequestTimeMicroseconds = new GUIContent("AverageTotalRequestTimeMicroseconds ", "The mean time taken from request to completion, in microseconds, for completed read request metrics included in the summary calculation.");
                    public static GUIContent AverageWaitTimeMicroseconds = new GUIContent("AverageWaitTimeMicroseconds ", "The mean time taken from request to the start of reading, in microseconds, for read request metrics included in the summary calculation.");

                    public static GUIContent LongestReadTimeMicroseconds = new GUIContent("LongestReadTimeMicroseconds ", "The longest read time(not including time in queue) included in the summary calculation in microseconds.");
                    public static GUIContent LongestWaitTimeMicroseconds = new GUIContent("LongestWaitTimeMicroseconds ", "The longest time spent waiting of metrics included in the summary calculation, in microseconds.");

                    public static GUIContent NumberOfAsyncReads = new GUIContent("NumberOfAsyncReads ", "The total number of Async reads in the metrics included in the summary calculation.");
                    public static GUIContent NumberOfCachedReads = new GUIContent("NumberOfCachedReads ", "The total number of cached reads(so read time was zero) in the metrics included in the summary calculation.");
                    public static GUIContent NumberOfCanceledRequests = new GUIContent("NumberOfCanceledRequests ", "");
                    public static GUIContent NumberOfCompletedRequests = new GUIContent("NumberOfCompletedRequests ", "");
                    public static GUIContent NumberOfFailedRequests = new GUIContent("NumberOfFailedRequests ", "");
                    public static GUIContent NumberOfInProgressRequests = new GUIContent("NumberOfInProgressRequests");
                    public static GUIContent NumberOfSyncReads = new GUIContent("NumberOfSyncReads", "The total number of Sync reads in the metrics included in the summary calculation.");
                    public static GUIContent NumberOfWaitingRequests = new GUIContent("NumberOfWaitingRequests ", "The total number of waiting requests in the metrics included in the summary calculation.");
                    
                    public static GUIContent TotalBytesRead = new GUIContent("TotalBytesRead ", "The total number of bytes read in the metrics included in the summary calculation");
                    public static GUIContent TotalNumberOfRequests = new GUIContent("TotalNumberOfRequests ", "The total number of bytes read in the metrics included in the summary calculation");
                }



                UTJ.AsyncReadManagerMetricsRemote.Metrics[] mMetrics;
                List<SummaryMetrics> mSummaryMetrics;
                Vector2 mScrollPos;
                int mSlider;
                bool [] mToggles;


                [MenuItem("Window/AsyncReadManagerMetricsRemote")]
                static void OpenWindow()
                {
                    var window = (AsyncReadManagerMetricsRemoteEditorWindow)EditorWindow.GetWindow(typeof(AsyncReadManagerMetricsRemoteEditorWindow));
                    window.titleContent = new GUIContent("ARMMRemote");
                }


                protected override void OnEnable()
                {
                    mSummaryMetrics = new List<SummaryMetrics>();
                    kMsgSendEditorToPlayer = new System.Guid("68b8adc1034c46f4b5149b668db907b1");
                    kMsgSendPlayerToEditor = new System.Guid("6751b3713d4d4d3d8cc2b31c157497b3");
                    base.remoteMessageCB = MessageReciveCB;
                    mScrollPos = Vector2.zero;
                    mSlider = 0;
                    mToggles = new bool[18];
                    for (var i = 0; i < mToggles.Length; i++) {
                        mToggles[i] = true;
                    }
                    
                    base.OnEnable();
                }


                private void OnGUI()
                {
                    UnityEditor.EditorGUILayout.BeginHorizontal();
                    ConnectionTargetSelectionDropdown();
                    if (GUILayout.Button("Start"))
                    {
                        var msg = new AsyncReadManagerMetricsRemoteMessageStart();
                        SendRemoteMessage(msg.ToBytes());
                    }
                    if (GUILayout.Button("Stop"))
                    {
                        var msg = new AsyncReadManagerMetricsRemoteMessageStop();
                        SendRemoteMessage(msg.ToBytes());
                    }
                    UnityEditor.EditorGUI.BeginDisabledGroup(mMetrics == null);
                    if (GUILayout.Button("Save"))
                    {
                        SaveMetrics();
                    }
                    UnityEditor.EditorGUI.EndDisabledGroup();
                    UnityEditor.EditorGUILayout.EndHorizontal();

                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();

                    UnityEditor.EditorGUILayout.BeginHorizontal();
                    UnityEditor.EditorGUILayout.LabelField("Summary");
                    if (GUILayout.Button("Reset"))
                    {
                        mSummaryMetrics.Clear();
                    }

                    UnityEditor.EditorGUI.BeginDisabledGroup(mSummaryMetrics.Count <= 0);
                    if (GUILayout.Button("Save"))
                    {
                        SaveMetrics();
                    }
                    UnityEditor.EditorGUI.EndDisabledGroup();
                    UnityEditor.EditorGUILayout.EndHorizontal();


                    if(mSummaryMetrics.Count <= 0)
                    {
                        return;
                    }

                    mScrollPos = UnityEditor.EditorGUILayout.BeginScrollView(mScrollPos);

                    var ofst = 0;
                    var count = 0;
                    
                    if (mSlider > (int)EditorGUIUtility.currentViewWidth)
                    {
                        ofst = mSlider - (int)EditorGUIUtility.currentViewWidth;
                        count = (int)EditorGUIUtility.currentViewWidth;
                    }
                    else
                    {
                        ofst = 0;
                        count = mSlider;
                    }

                    var protList = new List<float>();

                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add(mSummaryMetrics[i + ofst].AverageBandwidthMBPerSecond);                        
                    }
                    mToggles[0] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.AverageBandwidthMBPerSecond,mToggles[0]);
                    if(mToggles[0])
                        UTJ.EditorGUILayout.Graph(new GUIContent(),protList,GUILayout.Width((int)EditorGUIUtility.currentViewWidth),GUILayout.Height(200));                        
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();

                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add(mSummaryMetrics[i + ofst].AverageReadSizeInBytes);
                    }
                    mToggles[1] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.AverageReadSizeInBytes, mToggles[1]);
                    if (mToggles[1])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();

                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add(mSummaryMetrics[i + ofst].AverageReadTimeMicroseconds);
                    }
                    mToggles[2] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.AverageReadTimeMicroseconds, mToggles[2]);
                    if (mToggles[2])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].AverageThroughputMBPerSecond);
                    }
                    mToggles[3] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.AverageThroughputMBPerSeconds, mToggles[3]);
                    if (mToggles[3])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].AverageTotalRequestTimeMicroseconds);
                    }
                    mToggles[4] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.AverageTotalRequestTimeMicroseconds, mToggles[4]);
                    if (mToggles[4])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].AverageWaitTimeMicroseconds);
                    }
                    mToggles[5] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.AverageWaitTimeMicroseconds, mToggles[5]);
                    if (mToggles[5])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].LongestReadTimeMicroseconds);
                    }
                    mToggles[6] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.LongestReadTimeMicroseconds, mToggles[6]);
                    if (mToggles[6])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].LongestWaitTimeMicroseconds);
                    }
                    mToggles[7] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.LongestWaitTimeMicroseconds, mToggles[7]);
                    if (mToggles[7])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].NumberOfAsyncReads);
                    }
                    mToggles[8] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.NumberOfAsyncReads, mToggles[8]);
                    if (mToggles[8])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].NumberOfCachedReads);
                    }
                    mToggles[9] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.NumberOfCachedReads, mToggles[9]);
                    if (mToggles[9])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();

                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].NumberOfCanceledRequests);
                    }
                    mToggles[10] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.NumberOfCanceledRequests, mToggles[10]);
                    if (mToggles[10])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].NumberOfCompletedRequests);
                    }
                    mToggles[11] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.NumberOfCompletedRequests, mToggles[11]);
                    if (mToggles[11])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].NumberOfFailedRequests);
                    }
                    mToggles[12] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.NumberOfFailedRequests, mToggles[12]);
                    if (mToggles[12])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].NumberOfInProgressRequests);
                    }
                    mToggles[13] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.NumberOfInProgressRequests, mToggles[13]);
                    if (mToggles[13])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].NumberOfSyncReads);
                    }
                    mToggles[14] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.NumberOfSyncReads, mToggles[14]);
                    if (mToggles[14])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].NumberOfWaitingRequests);
                    }
                    mToggles[15] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.NumberOfWaitingRequests, mToggles[15]);
                    if (mToggles[15])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();

                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].TotalBytesRead);
                    }
                    mToggles[16] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.TotalBytesRead, mToggles[16]);
                    if (mToggles[16])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    protList.Clear();
                    for (var i = 0; i < count; i++)
                    {
                        protList.Add((float)mSummaryMetrics[i + ofst].TotalNumberOfRequests);
                    }
                    mToggles[17] = UnityEditor.EditorGUILayout.ToggleLeft(Styles.TotalNumberOfRequests, mToggles[17]);
                    if (mToggles[17])
                        UTJ.EditorGUILayout.Graph(new GUIContent(), protList, GUILayout.Width((int)EditorGUIUtility.currentViewWidth), GUILayout.Height(200));
                    UnityEditor.EditorGUILayout.Space();
                    UnityEditor.EditorGUILayout.Space();


                    UnityEditor.EditorGUILayout.EndScrollView();


                    UnityEditor.EditorGUILayout.Space();
                    // Frame選択用スライダー
                    mSlider = UnityEditor.EditorGUILayout.IntSlider(mSlider, 0, mSummaryMetrics.Count);
                }


                void MessageReciveCB(UTJ.RemoteConnect.Message remoteMessageBase)
                {
                    var id = (AsyncReadManagerMetricsRemoteMessage.MessageID)remoteMessageBase.messageId;

                    switch (id)
                    {
                        case AsyncReadManagerMetricsRemoteMessage.MessageID.GetMetrics:
                            {                                
                                var msg = (AsyncReadManagerMetricsRemoteMessageGetMetrics)remoteMessageBase;
                                mMetrics = msg.metrics;                                
                            }
                            break;

                        case AsyncReadManagerMetricsRemoteMessage.MessageID.GetSummaryMetrics:
                            {
                                var msg = (AsyncReadManagerMetricsRemoteMessageGetSummaryMetrics)remoteMessageBase;
                                mSummaryMetrics.Add(msg.summary);

                                mSlider = mSummaryMetrics.Count;
                                // 再描画
                                Repaint();
                            }
                            break;
                    }
                }


                void SaveSummary()
                {
                    var path = EditorUtility.SaveFilePanel("Save Summary Data as csv", "", "", "csv");
                    if (!string.IsNullOrEmpty(path))
                    {
                        using (var sw = new StreamWriter(path))
                        {
                            var sb = new StringBuilder();
                            sb.Append("AssetName,AssetTypeId,BatchReadCount,CurrentBytesRead,FileName,IsBatchRead,OffsetBytes,PriorityLevel,ReadType,RequestTimeMicroseconds,SizeBytes,State,Subsystem,TimeInQueueMicroseconds,TotalTimeMicroseconds");

                        }
                    }
                }


                void SaveMetrics()
                {
                    var path = EditorUtility.SaveFilePanel(
                    "Save Metrics Data as csv",
                    "",
                    "",
                    "csv");
                    if (!string.IsNullOrEmpty(path))
                    {
                        using (StreamWriter sw = new StreamWriter(path))
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(
                            "AverageBandwidthMBPerSecond,AverageReadSizeInBytesAverageReadTimeMicroseconds,AverageThroughputMBPerSecond,AverageTotalRequestTimeMicroseconds,AverageWaitTimeMicroseconds,LongestReadAssetType,LongestReadSubsystem,LongestReadTimeMicroseconds,LongestWaitAssetType,LongestWaitSubsystem,LongestWaitTimeMicroseconds,NumberOfAsyncReads,NumberOfCachedReads,NumberOfCanceledRequests,NumberOfCompletedRequests,NumberOfFailedRequests,NumberOfInProgressRequests,NumberOfSyncReads,NumberOfWaitingRequests,TotalBytesRead,TotalNumberOfRequests");


                            sw.WriteLine(sb);
                            foreach (var metric in mSummaryMetrics)
                            {
                                sb.Clear();
                                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{21}",
                                    metric.AverageBandwidthMBPerSecond,
                                    metric.AverageReadSizeInBytes,
                                    metric.AverageReadTimeMicroseconds,
                                    metric.AverageThroughputMBPerSecond,
                                    metric.AverageTotalRequestTimeMicroseconds,
                                    metric.AverageWaitTimeMicroseconds,
                                    metric.LongestReadAssetType,
                                    metric.LongestReadSubsystem,
                                    metric.LongestReadTimeMicroseconds,
                                    metric.LongestWaitAssetType,
                                    metric.LongestWaitSubsystem,
                                    metric.LongestWaitTimeMicroseconds,
                                    metric.NumberOfAsyncReads,
                                    metric.NumberOfCachedReads,
                                    metric.NumberOfCanceledRequests,
                                    metric.NumberOfCompletedRequests,
                                    metric.NumberOfFailedRequests,
                                    metric.NumberOfInProgressRequests,
                                    metric.NumberOfSyncReads,
                                    metric.NumberOfWaitingRequests,
                                    metric.TotalBytesRead,
                                    metric.TotalNumberOfRequests
                                    );
                                sw.WriteLine(sb);
                            }

                        }
                    }

                }
            }
        }
    }
}
#endif