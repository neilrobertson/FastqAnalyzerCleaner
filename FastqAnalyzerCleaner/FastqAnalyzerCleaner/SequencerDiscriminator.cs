﻿/// <copyright file="SequencerDescriminator.cs" author="Neil Robertson">
/// Copyright (c) 2013 All Right Reserved, Neil Alistair Robertson - neil.alistair.robertson@hotmail.co.uk
///
/// This code is the property of Neil Robertson.  Permission must be sought before reuse.
/// It has been written explicitly for the MRes Bioinfomatics course at the University 
/// of Glasgow, Scotland under the supervision of Derek Gatherer.
///
/// </copyright>
/// <author>Neil Robertson</author>
/// <email>neil.alistair.robertson@hotmail.co.uk</email>
/// <date>2013-06-1</date>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
	///<summary>
	///The SequencerDiscriminator class conforms to the Abstract Factory pattern and returns the desired sequencer class by building a Fq_FILE_MAP
	///of sequencer classes and allowing selection via string input
	///</summary>
    class SequencerDiscriminator
    {
	    public static Dictionary<String, ISequencer> storage = new Dictionary<String, ISequencer> ();
	    public static HashSet<String> checkExists = new HashSet<String>();
		
		private static Boolean isSetUp = false;
		///<summary>
		///Method allows the factory selection of a sequencer type class through the input of a string object that conforms to the class of that type
		///</summary>
		///<param name="sequencerName">The name of the sequencer type to select</param>
		///<return>An instance of that particular sequencer class type</return>
	    public static ISequencer getSequencerSpecifier(String sequencerName)
	    {
            if (isSetUp == false)
            {
                setUp();
                isSetUp = true;
            }
            if (sequencerName == null)
                return Sequencer_Default.sequencer as ISequencer;

		    ISequencer sequencer = (ISequencer) storage[sequencerName];
            if (sequencer == null)
                return Sequencer_Default.sequencer as ISequencer;

		    return sequencer;
	    }
	
		///<summary>
		///Called by individual sequencer type classes to register with the global Fq_FILE_MAP of types
		///</summary>
		///<param name="key">The sequencer type name</param>
		///<param name="value">A reference to a corresponding sequencer type initialized through the abstract class</param>
	    public static void register (String key, ISequencer value)
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
		    Sequencer_Default.register();
		    Sequencer_Sanger.register();
		    Sequencer_Solexa.register();
		    Sequencer_Illumina3.register();
		    Sequencer_Illumina5.register();
		    Sequencer_Illumina8.register();
		    Sequencer_Illumina9.register();
			isSetUp = true;
	    }
    }
}

