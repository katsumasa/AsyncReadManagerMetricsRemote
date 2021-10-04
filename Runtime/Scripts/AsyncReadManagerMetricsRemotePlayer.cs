using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO.LowLevel.Unsafe;
using UTJ.RemoteConnect;
using UTJ.AsyncReadManagerMetricsRemote;


namespace UTJ
{
    namespace AsyncReadManagerMetricsRemote
    {
        namespace Player
        {
            public class AsyncReadManagerMetricsRemotePlayer : UTJ.RemoteConnect.Player
            {
#if ENABLE_PROFILER && UNITY_2020_2_OR_NEWER
                static GameObject instance;
                [SerializeField] bool m_isDontDestroyOnLoad;

                AsyncReadManagerMetricsRemoteMessageGetSummaryMetrics asyncReadManagerMetricsRemoteMessageGetSummaryMetrics;
                List<UTJ.AsyncReadManagerMetricsRemote.Metrics> metricsList;



                private void Awake()
                {
                    Debug.Log("AsyncReadManagerMetricsRemotePlayer.Awake()");

                    if (instance == null)
                    {
                        instance = gameObject;
                        if (m_isDontDestroyOnLoad)
                        {
                            DontDestroyOnLoad(gameObject);
                        }
                    }
                    else
                    {
                        GameObject.Destroy(gameObject);
                    }
                }

                protected override void OnEnable()
                {
                    asyncReadManagerMetricsRemoteMessageGetSummaryMetrics = new AsyncReadManagerMetricsRemoteMessageGetSummaryMetrics();
                    metricsList = new List<Metrics>();

                    Debug.Log("AsyncReadManagerMetricsRemotePlayer.OnEnable()");
                    kMsgSendEditorToPlayer = new System.Guid("68b8adc1034c46f4b5149b668db907b1");
                    kMsgSendPlayerToEditor = new System.Guid("6751b3713d4d4d3d8cc2b31c157497b3");
                    remoteMessageCB = MessageReciveCB;
                    base.OnEnable();
                }


                protected override void OnDisable()
                {
                    Debug.Log("AsyncReadManagerMetricsRemotePlayer.OnDisable()");
                    if (AsyncReadManagerMetrics.IsEnabled())
                    {
                        AsyncReadManagerMetrics.StopCollectingMetrics();
                    }
                    metricsList.Clear();
                    base.OnDisable();
                }

                private void OnDestroy()
                {
                    Debug.Log("AsyncReadManagerMetricsRemotePlayer.OnDestroy()");

                    if (instance == gameObject)
                    {
                        instance = null;
                    }
                }


                private void Update()
                {
                    if (isRegist && AsyncReadManagerMetrics.IsEnabled())
                    {
                        var metrics = AsyncReadManagerMetrics.GetMetrics(AsyncReadManagerMetrics.Flags.None);
                        for (var i = 0; i < metrics.Length; i++)
                        {
                            metricsList.Add(new Metrics(metrics[i]));
                        }
                        asyncReadManagerMetricsRemoteMessageGetSummaryMetrics.summary.Set(AsyncReadManagerMetrics.GetCurrentSummaryMetrics(AsyncReadManagerMetrics.Flags.ClearOnRead));
                        SendRemoteMessage(asyncReadManagerMetricsRemoteMessageGetSummaryMetrics.ToBytes());
                    }
                }


                void MessageReciveCB(UTJ.RemoteConnect.Message remoteMessageBase)
                {
                    var id = (AsyncReadManagerMetricsRemoteMessage.MessageID)remoteMessageBase.messageId;

                    switch (id)
                    {
                        case AsyncReadManagerMetricsRemoteMessage.MessageID.Start:
                            {
                                // Debug.Log("AsyncReadManagerMetricsRemoteMessageStart()");
                                if (!AsyncReadManagerMetrics.IsEnabled())
                                {
                                    metricsList.Clear();
                                    AsyncReadManagerMetrics.StartCollectingMetrics();
                                    var msg = new AsyncReadManagerMetricsRemoteMessageStart();
                                    SendRemoteMessage(msg.ToBytes());
                                }
                            }
                            break;

                        case AsyncReadManagerMetricsRemoteMessage.MessageID.Stop:
                            {
                                // Debug.Log("AsyncReadManagerMetricsRemoteMessageStop()");
                                if (AsyncReadManagerMetrics.IsEnabled())
                                {
                                    var msg = new AsyncReadManagerMetricsRemoteMessageGetMetrics();
                                    AsyncReadManagerMetrics.StopCollectingMetrics();
                                    msg.metrics = metricsList.ToArray();
                                    metricsList.Clear();
                                    SendRemoteMessage(msg.ToBytes());
                                }
                            }
                            break;
                    }
                }
#endif
            }
        }
    }
}