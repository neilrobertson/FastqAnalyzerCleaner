using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
	///<summary>
	///The SequencerDiscriminator class conforms to the Abstract Factory pattern and returns the desired sequencer class by building a map
	///of sequencer classes and allowing selection via string input
	///</summary>
    class SequencerDiscriminator
    {
	    public static Dictionary<String, SequencerSpecifier> storage = new Dictionary<String, SequencerSpecifier> ();
	    public static HashSet<String> checkExists = new HashSet<String>();
		
		private static Boolean isSetUp = false;
		///<summary>
		///Method allows the factory selection of a sequencer type class through the input of a string object that conforms to the class of that type
		///</summary>
		///<param name="sequencerName">The name of the sequencer type to select</param>
		///<return>An instance of that particular sequencer class type</return>
	    public static SequencerSpecifier getSequencerSpecifier(String sequencerName)
	    {
			if (isSetUp == false)
				setUp();

		    SequencerSpecifier sequencer = (SequencerSpecifier) storage[sequencerName];
		    if (sequencer == null)
			    return DefaultSequencer.sequencer as SequencerSpecifier;
		    return sequencer;
	    }
	
		///<summary>
		///Called by individual sequencer type classes to register with the global map of types
		///</summary>
		///<param name="key">The sequencer type name</param>
		///<param name="value">A reference to a corresponding sequencer type initialized through the abstract class</param>
	    public static void register (String key, SequencerSpecifier value)
	    {
            if (checkExists.Contains(key) == false)
            {
                checkExists.Add(key);
                storage.Add(key, value); // guard against null keys
            }
	    }
	
		///<summary>
		///Called initially to ensure that all sequencer type classes have been initialized and registered with the factory
		///</summary>
	    private static void setUp()
	    {
		    DefaultSequencer.register();
		    SequencerSanger.register();
		    SequencerSolexa.register();
		    SequencerIllumina3.register();
		    SequencerIllumina5.register();
		    SequencerIllumina8.register();
		    SequencerIllumina9.register();
			isSetUp = true;
	    }
    }
}

