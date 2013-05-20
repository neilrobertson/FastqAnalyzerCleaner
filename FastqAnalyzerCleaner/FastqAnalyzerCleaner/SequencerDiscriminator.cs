using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    class SequencerDiscriminator
    {
	    public static Dictionary<String, SequencerSpecifier> storage = new Dictionary<String, SequencerSpecifier> ();
	    public static HashSet<String> checkExists = new HashSet<String>();

	    public static SequencerSpecifier getSequencerSpecifier(String sequencerName)
	    {
            setUp();

		    SequencerSpecifier sequencer = (SequencerSpecifier) storage[sequencerName];
		    if (sequencer == null)
			    return DefaultSequencer.sequencer as SequencerSpecifier;
		    return sequencer;
	    }
	
	    public static void register (String key, SequencerSpecifier value)
	    {
            if (checkExists.Contains(key) == false)
            {
                checkExists.Add(key);
                storage.Add(key, value); // guard against null keys
            }
	    }
	
	    private static void setUp()
	    {
		    DefaultSequencer.register();
		    SequencerSanger.register();
		    SequencerSolexa.register();
		    SequencerIllumina3.register();
		    SequencerIllumina5.register();
		    SequencerIllumina8.register();
		    SequencerIllumina9.register();
	    }
    }
}

