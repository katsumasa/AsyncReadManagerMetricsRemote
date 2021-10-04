using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO.LowLevel.Unsafe;
using UTJ.RemoteConnect;


namespace UTJ
{
    namespace AsyncReadManagerMetricsRemote
    {
            [System.Serializable]
            public class AsyncReadManagerMetricsRemoteMessage : UTJ.RemoteConnect.Message
            {
                public enum MessageID
                {
                    Start,
                    Stop,
                    GetMetrics,
                    GetSummaryMetrics,
                }

                /// <summary>
                /// 発生したシステム エラーを説明する人間が判読できる文字列
                /// </summary>
                [SerializeField] string mError;
                [SerializeField] bool mIsError;


                public string error
                {
                    get { return mError; }
                    set { mError = value; }
                }

                public bool isError
                {
                    get { return mIsError; }
                    set { mIsError = value; }
                }

                public AsyncReadManagerMetricsRemoteMessage(MessageID messageId) : base((int)messageId)
                {

                }
            }


            [System.Serializable]
            public class AsyncReadManagerMetricsRemoteMessageStart : AsyncReadManagerMetricsRemoteMessage
            {
                public AsyncReadManagerMetricsRemoteMessageStart() : base(MessageID.Start)
                {
                }
            }


            [System.Serializable]
            public class AsyncReadManagerMetricsRemoteMessageStop : AsyncReadManagerMetricsRemoteMessage
            {
                public AsyncReadManagerMetricsRemoteMessageStop() : base(MessageID.Stop)
                {
                }
            }


            [System.Serializable]
            public class Metrics
            {
                [SerializeField] public string AssetName;
                [SerializeField] public ulong AssetTypeId;
                [SerializeField] public uint BatchReadCount;
                [SerializeField] public ulong CurrentBytesRead;
                [SerializeField] public string FileName;
                [SerializeField] public bool IsBatchRead;
                [SerializeField] public ulong OffsetBytes;
#if UNITY_2020_1_OR_NEWER
                [SerializeField] public Unity.IO.LowLevel.Unsafe.Priority PriorityLevel;
                [SerializeField] public Unity.IO.LowLevel.Unsafe.FileReadType ReadType;
#endif
                [SerializeField] public double RequestTimeMicroseconds;
                [SerializeField] public ulong SizeBytes;
#if UNITY_2020_1_OR_NEWER
                [SerializeField] public Unity.IO.LowLevel.Unsafe.ProcessingState State;
                [SerializeField] public Unity.IO.LowLevel.Unsafe.AssetLoadingSubsystem Subsystem;
#endif
                [SerializeField] public double TimeInQueueMicroseconds;
                [SerializeField] public double TotalTimeMicroseconds;

#if UNITY_2020_1_OR_NEWER
                public Metrics(AsyncReadManagerRequestMetric asyncReadManagerRequestMetric)
                {
                    AssetName = asyncReadManagerRequestMetric.AssetName;
                    AssetTypeId = asyncReadManagerRequestMetric.AssetTypeId;
                    BatchReadCount = asyncReadManagerRequestMetric.BatchReadCount;
                    CurrentBytesRead = asyncReadManagerRequestMetric.CurrentBytesRead;
                    FileName = asyncReadManagerRequestMetric.FileName;
                    IsBatchRead = asyncReadManagerRequestMetric.IsBatchRead;
                    OffsetBytes = asyncReadManagerRequestMetric.OffsetBytes;
                    PriorityLevel = asyncReadManagerRequestMetric.PriorityLevel;
                    ReadType = asyncReadManagerRequestMetric.ReadType;
                    RequestTimeMicroseconds = asyncReadManagerRequestMetric.RequestTimeMicroseconds;
                    SizeBytes = asyncReadManagerRequestMetric.SizeBytes;
                    State = asyncReadManagerRequestMetric.State;
                    Subsystem = asyncReadManagerRequestMetric.Subsystem;
                    TimeInQueueMicroseconds = asyncReadManagerRequestMetric.TimeInQueueMicroseconds;
                    TotalTimeMicroseconds = asyncReadManagerRequestMetric.TotalTimeMicroseconds;
                }
#endif
            }


            [System.Serializable]
            public class AsyncReadManagerMetricsRemoteMessageGetMetrics : AsyncReadManagerMetricsRemoteMessage
            {
#if UNITY_2020_1_OR_NEWER
                [SerializeField] AsyncReadManagerMetrics.Flags mFlags;
#endif
                [SerializeField] Metrics[] mMetrics;


                public Metrics[] metrics
                {
                    get { return mMetrics; }
                    set { mMetrics = value; }
                }


#if UNITY_2020_1_OR_NEWER
                public AsyncReadManagerMetrics.Flags flags
                {
                    get { return mFlags; }
                    set { mFlags = value; }
                }
#endif

                public AsyncReadManagerMetricsRemoteMessageGetMetrics() : base(MessageID.GetMetrics)
                {
                    mMetrics = new Metrics[0];
                }
            }


            [System.Serializable]
            public class SummaryMetrics
            {
                [SerializeField] public float AverageBandwidthMBPerSecond;// The mean rate of reading of data(bandwidth), in Mbps, for read request metrics included in the summary calculation.
                [SerializeField] public float AverageReadSizeInBytes;  // The mean size of data read, in bytes, for read request metrics included in the summary calculation.
                [SerializeField] public float AverageReadTimeMicroseconds; // The mean time taken for reading(excluding queue time), in microseconds, for read request metrics included in the summary calculation.
                [SerializeField] public float AverageThroughputMBPerSecond;    // The mean rate of request throughput, in Mbps, for read request metrics included in the summary calculation.
                [SerializeField] public float AverageTotalRequestTimeMicroseconds; // The mean time taken from request to completion, in microseconds, for completed read request metrics included in the summary calculation.
                [SerializeField] public float AverageWaitTimeMicroseconds; // The mean time taken from request to the start of reading, in microseconds, for read request metrics included in the summary calculation.
                [SerializeField] public ulong LongestReadAssetType;    // The asset type ID for the longest read included in the summary calculation.
                [SerializeField] public Unity.IO.LowLevel.Unsafe.AssetLoadingSubsystem LongestReadSubsystem;    // The Subsystem tag for the longest read included in the summary calculation.
                [SerializeField] public float LongestReadTimeMicroseconds; // The longest read time(not including time in queue) included in the summary calculation in microseconds.
                [SerializeField] public ulong LongestWaitAssetType;   //  The asset type ID for the longest wait time included in the summary calculation.
                [SerializeField] public Unity.IO.LowLevel.Unsafe.AssetLoadingSubsystem LongestWaitSubsystem; // The Subsystem tag for the longest wait time included in the summary calculation.
                [SerializeField] public float LongestWaitTimeMicroseconds; // The longest time spent waiting of metrics included in the summary calculation, in microseconds.
                [SerializeField] public int NumberOfAsyncReads;  // The total number of Async reads in the metrics included in the summary calculation.
                [SerializeField] public int NumberOfCachedReads; // The total number of cached reads(so read time was zero) in the metrics included in the summary calculation.
                [SerializeField] public int NumberOfCanceledRequests;   // The total number of canceled requests in the metrics included in the summary calculation.
                [SerializeField] public int NumberOfCompletedRequests;   // The total number of completed requests in the metrics included in the summary calculation.
                [SerializeField] public int NumberOfFailedRequests;  // The total number of failed requests in the metrics included in the summary calculation.
                [SerializeField] public int NumberOfInProgressRequests;  // The total number of in progress requests in the metrics included in the summary calculation.
                [SerializeField] public int NumberOfSyncReads;   // The total number of Sync reads in the metrics included in the summary calculation.
                [SerializeField] public int NumberOfWaitingRequests; // The total number of waiting requests in the metrics included in the summary calculation.
                [SerializeField] public ulong TotalBytesRead;  // The total number of bytes read in the metrics included in the summary calculation.
                [SerializeField] public int TotalNumberOfRequests;


#if UNITY_2020_1_OR_NEWER
                /// <summary>
                /// 
                /// </summary>
                /// <param name="asyncReadManagerSummaryMetrics"></param>
                public SummaryMetrics(AsyncReadManagerSummaryMetrics asyncReadManagerSummaryMetrics)
                {
                    AverageBandwidthMBPerSecond = asyncReadManagerSummaryMetrics.AverageBandwidthMBPerSecond;
                    AverageReadSizeInBytes = asyncReadManagerSummaryMetrics.AverageReadSizeInBytes;
                    AverageReadTimeMicroseconds = asyncReadManagerSummaryMetrics.AverageReadTimeMicroseconds;
                    AverageThroughputMBPerSecond = asyncReadManagerSummaryMetrics.AverageThroughputMBPerSecond;
                    AverageTotalRequestTimeMicroseconds = asyncReadManagerSummaryMetrics.AverageTotalRequestTimeMicroseconds;
                    AverageWaitTimeMicroseconds = asyncReadManagerSummaryMetrics.AverageWaitTimeMicroseconds;
                    LongestReadAssetType = asyncReadManagerSummaryMetrics.LongestReadAssetType;
                    LongestReadSubsystem = asyncReadManagerSummaryMetrics.LongestReadSubsystem;
                    LongestReadTimeMicroseconds = asyncReadManagerSummaryMetrics.LongestReadTimeMicroseconds;
                    LongestWaitAssetType = asyncReadManagerSummaryMetrics.LongestWaitAssetType;
                    LongestWaitSubsystem = asyncReadManagerSummaryMetrics.LongestWaitSubsystem;
                    LongestWaitTimeMicroseconds = asyncReadManagerSummaryMetrics.LongestWaitTimeMicroseconds;
                    NumberOfAsyncReads = asyncReadManagerSummaryMetrics.NumberOfAsyncReads;
                    NumberOfCachedReads = asyncReadManagerSummaryMetrics.NumberOfCachedReads;
                    NumberOfCanceledRequests = asyncReadManagerSummaryMetrics.NumberOfCanceledRequests;
                    NumberOfCompletedRequests = asyncReadManagerSummaryMetrics.NumberOfCompletedRequests;
                    NumberOfFailedRequests = asyncReadManagerSummaryMetrics.NumberOfFailedRequests;
                    NumberOfInProgressRequests = asyncReadManagerSummaryMetrics.NumberOfInProgressRequests;
                    NumberOfSyncReads = asyncReadManagerSummaryMetrics.NumberOfSyncReads;
                    NumberOfWaitingRequests = asyncReadManagerSummaryMetrics.NumberOfWaitingRequests;
                    TotalBytesRead = asyncReadManagerSummaryMetrics.TotalBytesRead;
                    TotalNumberOfRequests = asyncReadManagerSummaryMetrics.TotalNumberOfRequests;
                }
#endif

                /// <summary>
                /// 
                /// </summary>
                public SummaryMetrics()
                {
                }

                public void Set(AsyncReadManagerSummaryMetrics asyncReadManagerSummaryMetrics)
                {
                    AverageBandwidthMBPerSecond = asyncReadManagerSummaryMetrics.AverageBandwidthMBPerSecond;
                    AverageReadSizeInBytes = asyncReadManagerSummaryMetrics.AverageReadSizeInBytes;
                    AverageReadTimeMicroseconds = asyncReadManagerSummaryMetrics.AverageReadTimeMicroseconds;
                    AverageThroughputMBPerSecond = asyncReadManagerSummaryMetrics.AverageThroughputMBPerSecond;
                    AverageTotalRequestTimeMicroseconds = asyncReadManagerSummaryMetrics.AverageTotalRequestTimeMicroseconds;
                    AverageWaitTimeMicroseconds = asyncReadManagerSummaryMetrics.AverageWaitTimeMicroseconds;
                    LongestReadAssetType = asyncReadManagerSummaryMetrics.LongestReadAssetType;
                    LongestReadSubsystem = asyncReadManagerSummaryMetrics.LongestReadSubsystem;
                    LongestReadTimeMicroseconds = asyncReadManagerSummaryMetrics.LongestReadTimeMicroseconds;
                    LongestWaitAssetType = asyncReadManagerSummaryMetrics.LongestWaitAssetType;
                    LongestWaitSubsystem = asyncReadManagerSummaryMetrics.LongestWaitSubsystem;
                    LongestWaitTimeMicroseconds = asyncReadManagerSummaryMetrics.LongestWaitTimeMicroseconds;
                    NumberOfAsyncReads = asyncReadManagerSummaryMetrics.NumberOfAsyncReads;
                    NumberOfCachedReads = asyncReadManagerSummaryMetrics.NumberOfCachedReads;
                    NumberOfCanceledRequests = asyncReadManagerSummaryMetrics.NumberOfCanceledRequests;
                    NumberOfCompletedRequests = asyncReadManagerSummaryMetrics.NumberOfCompletedRequests;
                    NumberOfFailedRequests = asyncReadManagerSummaryMetrics.NumberOfFailedRequests;
                    NumberOfInProgressRequests = asyncReadManagerSummaryMetrics.NumberOfInProgressRequests;
                    NumberOfSyncReads = asyncReadManagerSummaryMetrics.NumberOfSyncReads;
                    NumberOfWaitingRequests = asyncReadManagerSummaryMetrics.NumberOfWaitingRequests;
                    TotalBytesRead = asyncReadManagerSummaryMetrics.TotalBytesRead;
                    TotalNumberOfRequests = asyncReadManagerSummaryMetrics.TotalNumberOfRequests;
                }
            }


            [System.Serializable]
            public class AsyncReadManagerMetricsRemoteMessageGetSummaryMetrics : AsyncReadManagerMetricsRemoteMessage
            {
#if UNITY_2020_1_OR_NEWER
                [SerializeField] AsyncReadManagerMetrics.Flags mFlags;


                public AsyncReadManagerMetrics.Flags flags
                {
                    get { return mFlags; }
                    set { mFlags = value; }
                }
#endif
                [SerializeField] SummaryMetrics mSummary;


                public SummaryMetrics summary
                {
                    get { return mSummary; }
                    set { mSummary = value; }
                }


                public AsyncReadManagerMetricsRemoteMessageGetSummaryMetrics() : base(MessageID.GetSummaryMetrics)
                {
                    summary = new SummaryMetrics();
                }


#if UNITY_2020_1_OR_NEWER
                public AsyncReadManagerMetricsRemoteMessageGetSummaryMetrics(AsyncReadManagerSummaryMetrics asyncReadManagerSummaryMetrics) : base(MessageID.GetSummaryMetrics)
                {
                    summary = new SummaryMetrics(asyncReadManagerSummaryMetrics);
                }
#endif
            }
       
    }
}