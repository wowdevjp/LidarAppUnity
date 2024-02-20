using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UIElements;
using System.Security.Principal;

namespace Filter{
public class Filter1d
{
    private int windowSize;
    private int overThCnt;
    private int overThCntmax = 10;
    private List<int> idStack = new List<int>(10);
    // Filter1d(int _windowSize = 5){
    //     windowSize = _windowSize;
    // }
    void Awake()
    {
        idStack.Add(0);
    }

    public float[] clippingData(float[] _data, int minId, int maxId){
        List<float> _clipingData = new List<float>();
        for (int i = minId; i < maxId; i++)
        {
            _clipingData.Add(_data[i]);
        }
        return _clipingData.ToArray();
    }

/// <summary>
/// 移動平均フィルタ
/// </summary>
/// <param name="_data">実行するリスト</param>
/// <param name="_length">リスト長</param>
/// <param name="_windowSize">ウィンドウサイズ = 偶数</param>
    public void avarageFilter(ref float[] _data, int _length, int _windowSize){

        if (_windowSize%2 != 0)
        {
            Debug.LogAssertion("avarageFilter : _windowSize Error");
            Debug.Assert(true);
        }

        for (int i = _windowSize/2; i < _length - _windowSize/2; i++)
        {
            float _tmp = 0;
            for (int j = i - _windowSize/2; j < i + _windowSize/2; j++)
            {
                _tmp += _data[j];
            }
            _data[i] = _tmp/_windowSize;
        }
        for (int i = 0; i < _windowSize/2; i++)
        {
            _data[i] = _data[_windowSize/2];
            _data[_length - _windowSize/2 + i] = _data[_length - _windowSize/2];
        }
    }
    public float getAverage(float[] _data, int _length){
        return _data.Sum() / _length;
    }
    public int getAverage(int[] _data, int _length){
        return _data.Sum() / _length;
    }
/// <summary>
/// 勾配算出フィルタ
/// </summary>
/// <param name="_data"></param>
/// <param name="_length"></param>
/// <param name="_step"></param>
/// <returns></returns>
    public float[] gradFilter(float[] _data, int _length, int _step){
        float[] gradient = new float[2161];
        for (int i = _step/2; i < _length - _step/2; i++)
        {
            // 勾配計算
            gradient[i] = (_data[i + _step/2] - _data[i - _step/2])/_step;
        }   
        for (int i = 0; i < _step/2; i++)
        {
            gradient[i] = gradient[_step/2];
            gradient[_step - i] = gradient[_length - _step/2];
        }
        return gradient;
    }
    public int getMaxIndex(float[] _data, int _length, float th){
        int tmpId = 0;
        float tmpMax = th;
        for (int i = 0; i < _length; i++)
        {
            if (_data[i] < tmpMax)
            {
                tmpMax = _data[i];
                tmpId = i;
            }
        }
        //---th 以上の頂点を10カウント以上継続して取得したときに発火---
        if (tmpId == 0)
        {
            overThCnt = 0;
            idStack.Clear();
            Debug.Log($"Clear");
            return 0;
        }else if(overThCnt < overThCntmax){
            overThCnt++;
            Debug.LogAssertion($"{overThCntmax}");
            idStack.Add(tmpId);
            return 0;
        }else{
            idStack.RemoveAt(0);
            idStack.Insert(overThCntmax-1, tmpId);
            // int peakId = idStack.GroupBy(x => x).OrderByDescending(g => g.Count()).Select(g => g.Key).First();
            int peakId = getAverage(idStack.ToArray(), idStack.Count);
            return peakId;
        }
        //---th 以上の頂点を10カウント以上継続して取得したときに発火---
        
    }
    public List<int> getMaxList(float[] _data, float[] _gradList, int _length, int _width, float th){
        List<int> distanceList = new List<int>();
        for (int i = _width; i < _length - _width; i++)
        {
            int cnt = 0;
            if (_data[i] < th)
            {
                Debug.LogWarning($"Check Th:{th}, index:{i}, _data:{_data[i]}");
                for (int j = -_width/2; j < _width/2; j++)
                {
                    if (_gradList[i-j] > 0 && _gradList[i-j] < 0)
                    {
                        cnt += 1;
                    }
                }
                if (cnt > _width/4)
                {
                    distanceList.Add(i);
                    Debug.LogWarning($"MaxId:{i}");
                }
            }
        }
        return distanceList;
    }
}
// public List<float> edgeFilter(float[] _data, int _length, int _step){

//     bool flag = false;
// }
//     public void avarageFilter(ref float[] _data, int _length, int _windowSize){
//         Stack<float> windowStack = new Stack<float>();
//         // float[] windowStack = new float[_windowSize];
//         for (int i = 0; i < _length; i++)
//         {
//             if(i >= _windowSize - 1){
//                 windowStack.Pop();
//                 windowStack.Push(_data[i]);
//                 //平均値を代入
//                 float median1 = windowStack.OrderBy(x => x).ElementAt(windowStack.Count() / 2);
//                 _data[i] = Math.Abs(windowStack.Average());
//             }else{
//                 // windowサイズ分たまるのを待つ
//                 windowStack.Push(_data[i]);
//                 _data[i] = 0;
//             }
//         }
//         // for (int i = 0; i < _windowSize-1; i++)
//         // {
//         //     _data[i] = _data[_length - 1];
//         // }
//     }
// }

}

// public static class ListExtensions
// {
//     /// <summary>
//     /// 先頭にあるオブジェクトを削除せずに返します
//     /// </summary>
//     public static T Peek<T>( this IList<T> self )
//     {
//         return self[ 0 ];
//     }

//     /// <summary>
//     /// 先頭にあるオブジェクトを削除し、返します
//     /// </summary>
//     public static T Pop<T>( this IList<T> self )
//     {
//         var result = self[ 0 ];
//         self.RemoveAt( 0 );
//         return result;
//     }

//     /// <summary>
//     /// 末尾にオブジェクトを追加します
//     /// </summary>
//     public static void Push<T>( this IList<T> self, T item )
//     {
//         self.Insert( 0, item );
//     }
// }