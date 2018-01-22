using GoStreamAudioLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoStreamAudioGUI
{
    public static class PlayListShuffler
    {
        /// <summary>
        /// returns the computed track order to play from playlist
        /// </summary>
        /// <param name="items">the items in the playlist</param>
        /// <returns></returns>
        public static List<PlayListEntry> ComputedRandomOrder(System.Windows.Forms.ListView.ListViewItemCollection items)
        {
            List<PlayListEntry> pltems = null;
            if (items != null)
            {
                int listSize = items.Count;
                if (listSize > 0)
                {
                    pltems = new List<PlayListEntry>();
                    List<int> order = new List<int>();
                    PlayListEntry entry = new PlayListEntry();                     
                    Random _rand = new Random(DateTime.Now.Millisecond);
                    var possible = Enumerable.Range(0, listSize).ToList();
                    /*var possible = Enumerable.Range(0, listSize)
                            .Select(r => _rand.Next(listSize))
                            .ToList();*/
                    //List<int> listNumbers = new List<int>();
                    for (int i = 0; i < listSize; i++)
                    {
                        int index = _rand.Next(0, possible.Count);
                        order.Add(possible[index]);
                        possible.RemoveAt(index);                        
                    }
                    foreach (int pos in order)
                    {
                        if (items[pos].Text != "")
                        {
                            AudioFileInfo afInfo = new AudioFileInfo(items[pos].SubItems[0].Text, items[pos].SubItems[2].Text);

                            PlayListEntry plIt = new PlayListEntry
                            {
                                FileName = afInfo.FullPath,
                                HasPlayedOnce = false,
                                PosInPlayList = pos
                            };
                            pltems.Add(plIt);
                        }
                    }
                }
            }
            return pltems;
        }
    }
}
