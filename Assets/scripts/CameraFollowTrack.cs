using UnityEngine;
using UnityEngine.Timeline;

[TrackColor(0.3f, 0.6f, 0.9f)] // 可选：设置轨道颜色
[TrackClipType(typeof(CameraFollowAsset))] // 指定此轨道可以使用的剪辑类型
public class CameraFollowTrack : TrackAsset
{
    // 可以留空，Timeline 会自动处理 PlayableAsset 的创建和播放
}