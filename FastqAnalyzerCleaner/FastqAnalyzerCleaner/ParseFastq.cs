/// <copyright file="ParseFastq.cs" author="Neil Robertson">
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FastqAnalyzerCleaner
{
    public class ParseFastq
    {
        private String fastqHeader, fileName;
	
	    private Boolean IsFastqFile = false;
        private Boolean IsStandardFormat = false;
        private Boolean IsMultiLineFormat = false;

        private readonly int FQ_BLOCKS_TO_CHECK = 50;

        private String[] header, seq, seq2, info, qscore, qscore2;

	    private IFqFile fastqFile;
	    private FqSequence fqSeq;

        private long fileLength;
        private byte[] byteArray;
        private int lineCount = 0;
        private String FILE_FORMAT_TYPE;

        private Stopwatch sw;

        public static readonly char CARRIDGE_RETURN = '\n';
        public static readonly String REPORT_STATEMENT = "[PARSING FILE]";

        private FileStream fileReader;

	    public ParseFastq(FileStream fileReader, String fileName)
	    {
            this.fileName = fileName;
            this.fileReader = fileReader;
            this.fileLength = fileReader.Length;

            Console.WriteLine("File name: {0} Size: {1}MB OPENED", fileName, HelperMethods.ConvertBytesToMegabytes(fileLength).ToString("0.00"));

            this.IsFastqFile = CheckFastqFile();
            CountLines();
	    }

        public void CountLines()
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            BufferedStream bs = new BufferedStream(fileReader);
            StreamReader reader = new StreamReader(bs);
            
            while ((reader.ReadLine()) != null)
            {
                lineCount++;
            }
            fileReader.Seek(0, SeekOrigin.Begin);
            stw.Stop();
            Console.WriteLine("Checked Lines: {0} in T: {1}s", lineCount, stw.Elapsed);
        }

        public void InitByteParseFastq()
        {
            sw = new Stopwatch();
            sw.Start();

            try
            {
                byteArray = new byte[fileLength];

                while (fileLength > 0)
                {
                    if (fileLength < byteArray.Length)
                    {
                        fileReader.Read(byteArray, 0, (int)fileLength);
                        break;
                    }
                    else
                    {
                        fileReader.Read(byteArray, 0, byteArray.Length);
                    }
                    fileLength -= byteArray.Length;
                }
            }
            catch (InsufficientMemoryException exception)
            {
                Console.WriteLine(exception.StackTrace);
                UserResponse.ErrorResponse(exception.ToString());
            }
            catch (OutOfMemoryException exception)
            {
                Console.WriteLine(exception.StackTrace);
                UserResponse.ErrorResponse(exception.ToString());
            }
            catch (OverflowException exception)
            {
                Console.WriteLine(exception.StackTrace);
                UserResponse.ErrorResponse(exception.ToString());
            }
            catch(IOException exception)
            {
                Console.WriteLine(exception.ToString());
                UserResponse.ErrorResponse(exception.ToString());
            }
            finally
            {
                fileReader.Close();
            }

            Console.WriteLine("Read file to byte array. Time: {0} ms", sw.ElapsedMilliseconds);
        }

        public IFqFile Parse()
	    {
            if (IsStandardFormat == true)
            {
                FILE_FORMAT_TYPE = "Standard Fastq File Format";
                fastqFile = ParseStandardFormat();
            }
            else if (IsMultiLineFormat == true)
            {
                FILE_FORMAT_TYPE = "Multi-Line Fastq File Format";
                fastqFile = ParseMultiLineFormat();
            }
            return fastqFile;
	    }

        public IEnumerable<FqFile_Component> ParseComponents()
        {
            if (IsStandardFormat == true)
            {
                FILE_FORMAT_TYPE = "Standard Fastq File Format";
                foreach (FqFile_Component component in ParseStandardFormatComponent())
                {
                    yield return component;
                }
            }
            else if (IsMultiLineFormat == true)
            {
                FILE_FORMAT_TYPE = "Multi-Line Fastq File Format";
                foreach (FqFile_Component component in ParseMultiLineFormatComponent())
                {
                    yield return component;
                }
            }    
        }

        public IFqFile ParseStandardFormat()
        {
            sw = new Stopwatch();
            sw.Start();
            int seqIndex = 0;

            BufferedStream bs;
            StreamReader reader;

            try
            {
                bs = new BufferedStream(fileReader);
                reader = new StreamReader(bs);

                if (IsFastqFile == true)
                {

                    FqNucleotideRead fqRead = new FqNucleotideRead(' ', ' ');
                    //fastqFile = FqFileSpecifier.getInstance().getFqFile(Preferences.getInstance().getMultiCoreProcessing());
                    fastqFile.setFastqFileName(fileName);

                    long nLine = -1L;

                    while ((fastqHeader = reader.ReadLine()) != null)
                    {
                        nLine++;

                        if (nLine % 4 != 0) continue;

                        String seqlist = reader.ReadLine();
                        String infoHeader = reader.ReadLine();
                        String qscore = reader.ReadLine();

                        fqSeq = new FqSequence(seqIndex, fastqHeader, infoHeader, seqlist.Length);
                        seqIndex++;

                        for (int i = 0; i < (seqlist.Length); i++)
                        {
                            fqRead.resetFqNucleotideRead(seqlist[i], qscore[i]);
                            int hashcode = fqRead.hashcode();
                            fqSeq.addNucleotideRead(hashcode);
                        }
                        fastqFile.addFastqSequence(fqSeq);
                        nLine += 3;
                    }
                    sw.Stop();
                    Console.WriteLine("Time to Parse File:  " + sw.Elapsed + "s");
                }
            }
            finally
            {
                fileReader.Close();
            }
            return fastqFile;
        }

        public IEnumerable<FqFile_Component> ParseStandardFormatComponent()
        {
            sw = new Stopwatch();
            sw.Start();
            int seqIndex = 0;
            int blockNumber = 0;

            BufferedStream bs;
            StreamReader reader;

            try
            {
                bs = new BufferedStream(fileReader);
                reader = new StreamReader(bs);
  
                if (IsFastqFile == true)
                {
                    FqNucleotideRead fqRead = new FqNucleotideRead(' ', ' ');
                    FqFile_Component fastqFileComponent = new FqFile_Component();

                    long nLine = -1L;

                    while ((fastqHeader = reader.ReadLine()) != null)
                    {
                        nLine++;

                        if (nLine % 4 != 0) continue;

                        String seqlist = reader.ReadLine();
                        String infoHeader = reader.ReadLine();
                        String qscore = reader.ReadLine();

                        fqSeq = new FqSequence(seqIndex, fastqHeader, infoHeader, seqlist.Length);
                        seqIndex++;

                        for (int i = 0; i < (seqlist.Length); i++)
                        {
                            fqRead.resetFqNucleotideRead(seqlist[i], qscore[i]);
                            int hashcode = fqRead.hashcode();
                            fqSeq.addNucleotideRead(hashcode);
                        }
                        fastqFileComponent.addFastqSequence(fqSeq);
                        nLine += 3;
                        blockNumber ++;
                        if (blockNumber == FqFileMap.FQ_BLOCK_LIMIT)
                        {
                            yield return fastqFileComponent;
                            
                            blockNumber = 0;
                            fastqFileComponent = new FqFile_Component();
                        }
                    }
                    yield return fastqFileComponent; 
                    sw.Stop();
                    Console.WriteLine("Time to Parse File:  " + sw.Elapsed + "s");
                }
            }
            finally
            {
                fileReader.Close();
            }
        }

        public IFqFile ParseMultiLineFormat()
        {
            sw = new Stopwatch();
            sw.Start();
            int seqIndex = 0;

            BufferedStream bs;
            StreamReader reader;
           
            bs = new BufferedStream(fileReader);
            reader = new StreamReader(bs);

            if (IsFastqFile == true)
            {
                FqNucleotideRead fqRead = new FqNucleotideRead(' ', ' ');
                //fastqFile = FqFileSpecifier.getInstance().getFqFile(Preferences.getInstance().getMultiCoreProcessing());
                fastqFile.setFastqFileName(fileName);

                long nLine = -1L;

                while ((fastqHeader = reader.ReadLine()) != null)
                {
                    nLine++;

                    if (nLine % 6 != 0) continue;

                    String seqlist = reader.ReadLine();
                    String seqlist2 = reader.ReadLine();
                    String infoHeader = reader.ReadLine();
                    String qscore = reader.ReadLine();
                    String qscore2 = reader.ReadLine();

                    fqSeq = new FqSequence(seqIndex, fastqHeader, infoHeader, (seqlist.Length + seqlist2.Length));
                    seqIndex++;
 
                    for (int i = 0; i < (seqlist.Length); i++)
                    {
                        fqRead.resetFqNucleotideRead(seqlist[i], qscore[i]);
                        int hashcode = fqRead.hashcode();
                        fqSeq.addNucleotideRead(hashcode);
                    }

                    if (seqlist2.Length > 0 && (seqlist2.Length == qscore2.Length))
                    {
                        for (int i = 0; i < (seqlist.Length); i++)
                        {
                            fqRead.resetFqNucleotideRead(seqlist2[i], qscore2[i]);
                            int hashcode = fqRead.hashcode();
                            fqSeq.addNucleotideRead(hashcode);
                        }
                    }
                    fastqFile.addFastqSequence(fqSeq);
                    nLine += 3;
                }
                sw.Stop();
                Console.WriteLine("Time to Parse File:  " + sw.Elapsed + "s");
            }
            
            return fastqFile;
        }

        public IEnumerable<FqFile_Component> ParseMultiLineFormatComponent()
        {
            sw = new Stopwatch();
            sw.Start();
            int seqIndex = 0;
            int blockNumber = 0;

            BufferedStream bs;
            StreamReader reader;

            try
            {
                bs = new BufferedStream(fileReader);
                reader = new StreamReader(bs);

                if (IsFastqFile == true)
                {
                    FqNucleotideRead fqRead = new FqNucleotideRead(' ', ' ');
                    FqFile_Component fastqFileComponent = new FqFile_Component();

                    long nLine = -1L;

                    while ((fastqHeader = reader.ReadLine()) != null)
                    {
                        nLine++;

                        if (nLine % 6 != 0) continue;

                        String seqlist = reader.ReadLine();
                        String seqlist2 = reader.ReadLine();
                        String infoHeader = reader.ReadLine();
                        String qscore = reader.ReadLine();
                        String qscore2 = reader.ReadLine();

                        fqSeq = new FqSequence(seqIndex, fastqHeader, infoHeader, (seqlist.Length + seqlist2.Length));
                        seqIndex++;

                        for (int i = 0; i < (seqlist.Length); i++)
                        {
                            fqRead.resetFqNucleotideRead(seqlist[i], qscore[i]);
                            int hashcode = fqRead.hashcode();
                            fqSeq.addNucleotideRead(hashcode);
                        }

                        if (seqlist2.Length > 0 && (seqlist2.Length == qscore2.Length))
                        {
                            for (int i = 0; i < (seqlist.Length); i++)
                            {
                                fqRead.resetFqNucleotideRead(seqlist2[i], qscore2[i]);
                                int hashcode = fqRead.hashcode();
                                fqSeq.addNucleotideRead(hashcode);
                            }
                        }
                        fastqFile.addFastqSequence(fqSeq);
                        nLine += 3;
                        blockNumber++;
                        if (blockNumber == FqFileMap.FQ_BLOCK_LIMIT)
                        {
                            yield return fastqFileComponent;

                            blockNumber = 0;
                            fastqFileComponent = new FqFile_Component();
                        }
                    }
                    yield return fastqFileComponent;
                }
            }
            finally
            {
                fileReader.Close();
            }   
            sw.Stop();
            Console.WriteLine("Time to Parse File:  " + sw.Elapsed + "s");
        }


        public IFqFile parseByteFastq()
        {
            //fastqFile = FqFileSpecifier.getInstance().getFqFile(Preferences.getInstance().getMultiCoreProcessing());
            fastqFile.setFastqFileName(fileName);

            FqNucleotideRead fqRead = new FqNucleotideRead();
            FqSequence fqSeq;

            sw = new Stopwatch();
            sw.Start();
            
            const int LINES_IN_BLOCK = 4, HEADER_LINE = 0, SEQLINE = 1, INFO_LINE = 2, QUAL_LINE = 3, MAX_LINE = 250;
            int lineIter = 0, bitIter = 1, seqIndex = 0;
            byte[][] fqBlocks = new byte[4][]
            {
                new byte[MAX_LINE],
                new byte[MAX_LINE],
                new byte[MAX_LINE],
                new byte[MAX_LINE]
            };

            for (int i = 0; i < byteArray.Length; i++)
            {
                if (byteArray[i] == CARRIDGE_RETURN)
                {
                    fqBlocks[lineIter][0] = (byte) bitIter;
                    bitIter = 1;
                    lineIter++;
                    if (lineIter == LINES_IN_BLOCK)
                    {
                        lineIter = 0;
                        fqSeq = new FqSequence(seqIndex, System.Text.Encoding.ASCII.GetString(fqBlocks[HEADER_LINE]), System.Text.Encoding.ASCII.GetString(fqBlocks[INFO_LINE]), fqBlocks[SEQLINE][0]);
                        for (int j = 1; j < (fqBlocks[SEQLINE][0]); j++)
                        {
                            fqRead.resetFqNucleotideRead((char)fqBlocks[SEQLINE][j], (char)fqBlocks[QUAL_LINE][j]);
                            int hashcode = fqRead.hashcode();
                            fqSeq.addNucleotideRead(hashcode);
                        }
                        fastqFile.addFastqSequence(fqSeq);
                        seqIndex++;
                    }
                }
                else
                {
                    fqBlocks[lineIter][bitIter] = byteArray[i];
                    bitIter++;
                }
            }
            Console.WriteLine("PARSED: File read in {0} s from byte array", sw.Elapsed);
            return fastqFile;
        }
	
	    /// <summary>
	    /// Method checks the validity of the fastq file
	    /// </summary>
	    /// <param name="reader">The input stream</param>
	    /// <returns>True if the check determines this is a valid fastq file</returns>
	    public Boolean CheckFastqFile()
        {
            IsStandardFormat = CheckStandardFormat();

            if (IsStandardFormat == true)
                return true;

            IsMultiLineFormat = CheckMultiLineFormat();

            if (IsMultiLineFormat == true)
                return true;

            return false;
	    }

        public Boolean CheckStandardFormat()
        {
            header = new String[FQ_BLOCKS_TO_CHECK];
            seq = new String[FQ_BLOCKS_TO_CHECK];
            info = new String[FQ_BLOCKS_TO_CHECK];
            qscore = new String[FQ_BLOCKS_TO_CHECK];

            BufferedStream bs = new BufferedStream(fileReader);
            StreamReader reader = new StreamReader(bs);

            // Build sets of fastq blocks to check
            for (int i = 0; i < FQ_BLOCKS_TO_CHECK; i++)
            {
                header[i] = reader.ReadLine();
                seq[i] = reader.ReadLine();
                info[i] = reader.ReadLine();
                qscore[i] = reader.ReadLine();

            }

            // Check sets
            for (int i = 0; i < FQ_BLOCKS_TO_CHECK; i++)
            {
                if ((char)header[i][0] != IFqFile.INITIAL_HEADER_CHARACTER) return false;
                if ((char)info[i][0] != IFqFile.INITIAL_INFO_LINE_CHARACTER) return false;
                if (Nucleotide.isDNA(seq[i]) != true) return false;
                if (seq[i].Length != qscore[i].Length) return false;
            }
            
            fileReader.Seek(0, SeekOrigin.Begin);
            Console.WriteLine("FileReader Position: {0}", fileReader.Position);
            Console.WriteLine("File is found to be a fastqFile");
            return true;
        }


        private Boolean CheckMultiLineFormat()
        {
            Console.WriteLine("Checking Multiline formatted file type");
            
            header = new String[FQ_BLOCKS_TO_CHECK];
            seq = new String[FQ_BLOCKS_TO_CHECK];
            seq2 = new String[FQ_BLOCKS_TO_CHECK];
            info = new String[FQ_BLOCKS_TO_CHECK];
            qscore = new String[FQ_BLOCKS_TO_CHECK];
            qscore2 = new String[FQ_BLOCKS_TO_CHECK];

            BufferedStream bs = new BufferedStream(fileReader);
            StreamReader reader = new StreamReader(bs);

            // Build sets of fastq blocks to check
            for (int i = 0; i < FQ_BLOCKS_TO_CHECK; i++)
            {
                header[i] = reader.ReadLine();
                seq[i] = reader.ReadLine();
                seq2[i] = reader.ReadLine();
                info[i] = reader.ReadLine();
                qscore[i] = reader.ReadLine();
                qscore2[i] = reader.ReadLine();
            }

            // Check sets
            for (int i = 0; i < FQ_BLOCKS_TO_CHECK; i++)
            {
                if ((char)header[i][0] == IFqFile.INITIAL_HEADER_CHARACTER && (qscore[i] == null || qscore[i] == "")) return false;
                if ((char)header[i][0] != IFqFile.INITIAL_HEADER_CHARACTER) return false;
                if ((char)info[i][0] != IFqFile.INITIAL_INFO_LINE_CHARACTER) return false;
                if (Nucleotide.isDNA(seq[i]) != true) return false;
                if (seq[i].Length != qscore[i].Length) return false;
                if (Nucleotide.isDNA(seq2[i]) != true) return false;
                if (seq2[i].Length != qscore2[i].Length) return false;
            }
            
            fileReader.Seek(0, SeekOrigin.Begin);
            Console.WriteLine("FileReader Position: {0}", fileReader.Position);
            Console.WriteLine("File is found to be a fastqFile");
            return true;
        }
	
	    public Boolean getFastqFileCheck()
	    {
		    return IsFastqFile;
	    }

        public void CloseReader()
        {
            fileReader.Close();
            fileReader.Dispose();
        }

        public long GetFastqFileLength()
        {
            return fileLength;
        }

        public int GetLineCount()
        {
            return lineCount;
        }

        public String GetFileFormatType()
        {
            return FILE_FORMAT_TYPE;
        }
    }
}
