using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FastqAnalyzerCleaner
{
    class ParseFastq
    {
        private String fastqHeader, fileName;
	
        private Stopwatch stopwatch = new Stopwatch();
	
	    private Boolean isFastqFile = true;
        private readonly int FQ_BLOCKS_TO_CHECK = 10;
        private String[] header, seq, info, qscore;

	    private FqFile fastqFile;
	    private FqSequence fqSeq;

        private long _fileLength;
        private byte[] byteArray;

        private Stopwatch sw;

        private FileStream fileReader;

	    public ParseFastq(FileStream fileReader, String fileName)
	    {
            this.fileName = fileName;
            this.fileReader = fileReader;
	    }

        public void initByteParseFastq()
        {           
            _fileLength = fileReader.Length;

            byteArray = new byte[_fileLength];

            Console.WriteLine("FILE BYTES: " + _fileLength);

            sw = new Stopwatch();
            sw.Start();

            try
            {
                while (_fileLength > 0)
                {
                    if (_fileLength < byteArray.Length)
                    {
                        fileReader.Read(byteArray, 0, (int)_fileLength);
                        break;
                    }
                    else
                    {
                        fileReader.Read(byteArray, 0, byteArray.Length);
                    }
                    _fileLength -= byteArray.Length;
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
                Console.WriteLine(exception.StackTrace);
                UserResponse.ErrorResponse(exception.ToString());
            }
            finally
            {
                fileReader.Close();
            }

            Console.WriteLine("Time: {0} ms", sw.ElapsedMilliseconds);

        }

        public FqFile parse()
	    {
            sw = new Stopwatch();
            sw.Start();
            int seqIndex = 0;

	        //isFastqFile = checkFastqFile();

            BufferedStream bs;
            StreamReader reader;

            try
            {
                bs = new BufferedStream(fileReader);
                reader = new StreamReader(bs);
                if (isFastqFile == true)
                {

                    FqNucleotideRead fqRead = new FqNucleotideRead(' ', ' ');
                    fastqFile = FqFileSpecifier.getInstance().getFqFile(Preferences.getInstance().getMultiCoreProcessing()); 
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
                    Console.Write("Time to Parse File:  " + sw.Elapsed + "s" + "\n");
                }
            }
            catch (IOException exception)
            {
                Console.Write(exception.StackTrace);
                UserResponse.ErrorResponse(exception.ToString());
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
            finally
            {
                fileReader.Close();
            }
            return fastqFile;
	    }

        public FqFile parseByteFastq()
        {
            byte a = new byte();
            Console.WriteLine("A:" + a);
            fastqFile = FqFileSpecifier.getInstance().getFqFile(Preferences.getInstance().getMultiCoreProcessing());
            fastqFile.setFastqFileName(fileName);
            FqNucleotideRead fqRead = new FqNucleotideRead(' ', ' ');
            FqSequence fqSeq;
            sw = new Stopwatch();
            sw.Start();
            try
            {
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
                    if (byteArray[i] == '\n')
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
            }
            catch (OutOfMemoryException exception)
            {
                Console.WriteLine(exception.StackTrace);
                UserResponse.ErrorResponse(exception.ToString());
            }
            Console.WriteLine("PARSED: File read in {0} s from byte array", sw.Elapsed);
            return fastqFile;
        }
	
	    /// <summary>
	    /// Method checks the validity of the fastq file
	    /// </summary>
	    /// <param name="reader">The input stream</param>
	    /// <returns>True if the check determines this is a valid fastq file</returns>
	    public Boolean checkFastqFile()
        {
           Nucleotide nucleotide = new Nucleotide();

           header = new String[FQ_BLOCKS_TO_CHECK]; 
           seq = new String[FQ_BLOCKS_TO_CHECK];
           info = new String[FQ_BLOCKS_TO_CHECK];
           qscore = new String[FQ_BLOCKS_TO_CHECK];
            
            try
            {

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
                    Console.WriteLine(header[0][i]);
                    if ((char)header[0][i] != '@') return false;
                    if ((char)info[i][0] != '+') return false;
                    if (nucleotide.isDNA(seq[i]) != true) return false;
                    for (int j = 0; j < FQ_BLOCKS_TO_CHECK; j++)
                        if (seq[i].Length != qscore[j].Length) return false;
                }
            }
            catch (IOException exception)
            {
                Console.Write(exception.StackTrace);
                UserResponse.ErrorResponse(exception.ToString());
            }
                
            fileReader.Seek(0, SeekOrigin.Begin);
		    return true;
	    }
	
	    public Boolean getFastqFileCheck()
	    {
		    return isFastqFile;
	    }
    }
}
