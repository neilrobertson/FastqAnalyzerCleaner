using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class TaskDiscrimination
    {
        public static Dictionary<String, ITaskStrategy> storage = new Dictionary<String, ITaskStrategy>();
        public static HashSet<String> checkExists = new HashSet<String>();
        public static Boolean isSetUp = false;
        public static ITaskStrategy getTask(String taskName)
        {
            if (isSetUp == false)
            {
                setUp();
                isSetUp = true;
            }

            ITaskStrategy task = (ITaskStrategy)storage[taskName];
            if (task == null)
                return Task_DefaultTask.task as ITaskStrategy;
            return task;
        }

        public static void register(String key, ITaskStrategy value)
        {
            if (checkExists.Contains(key) == false)
            {
                checkExists.Add(key);
                storage.Add(key, value);
            }
        }

        private static void setUp()
        {
            Task_DefaultTask.register();
            Task_SequenceStatisticsTask.register();
            Task_EndCleanTask.register();
            Task_StartCleanTask.register();
            Task_TailCleanTask.register();
            Task_ReanalyzeTask.register();
            Task_RescanSequencerTask.register();
            Task_AdapterTask.register();
            Task_FindSequences.register();
            Task_LoadTask.register();
            Task_RemoveBelowMeanThreshold.register();
            Task_RemoveMisSeqeuence.register();
            Task_SaveCSV.register();
            Task_SaveFastq.register();
        }
    }
}
