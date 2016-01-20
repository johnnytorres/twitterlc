# -*- coding: utf-8 -*-





def processfile(inputfile, bUseWeights):
    
    # prepare tokens creator
    tokenizer = RegexpTokenizer(r'\w+')
    
    # create English stop words list
    es_stop = get_stop_words(language)
    
    # create p_stemmer of class PorterStemmer
    p_stemmer = SnowballStemmer(language) 
    #p_stemmer = PorterStemmer()
    
    # create documents list
    doc_set =[]
    weight_set =[]
    sumWeights = 0.0
    numDocs = 0
    weigths_file =inputfile+'.weights'
    usewfile= os.path.exists(weigths_file) and bUseWeights
    logging.info("loading tweets...")
    logging.info("using weightings: " + str(usewfile))
    
    fsource = open(inputfile,'r')
    
    for line in fsource:
        doc_set.append(line)
        numDocs+=1
        if not usewfile:
            w = 1
            sumWeights += w
            weight_set.append(w)
        
    fsource.close()
    
    if usewfile:
        logging.info("loading tweets weights...")
        
        fsource = open(weigths_file,'r')
        
        for line in fsource:
            w = int(line)
            w = log(w,2)
            sumWeights += w
            weight_set.append(w)
             
        fsource.close()
    
    
    # create tokenized list
    logging.info("data cleansing...")
    texts=[]
    
    for i in doc_set:
       
       # clean and tokenize document string
       raw=i
       tokens=tokenizer.tokenize(raw)
       # remove stops words from tokens
       stopped_tokens = [i for i in tokens if not i in es_stop]
       # stem tokens
       stemmed_tokens = [i for i in stopped_tokens]#p_stemmer.stem(i)
       # add tokens to list
       texts.append(stemmed_tokens)
       #print(stemmed_tokens)
      
    '''
    #remove words that appear only once
    from collections import defaultdict
    frequency = defaultdict(int)
    for text in texts:
         for token in text:
             frequency[token] += 1
      
    texts = [[token for token in text if frequency[token] > 1] for text in texts]
    ''' 
     
    for t in texts:
         print (t)
       
    logging.info("turn our tokenized documents into a id <-> term dictionary")
    dictionary = corpora.Dictionary(texts)     
    
    
    logging.info("convert tokenized documents into a document-term matrix")
    corpus = [dictionary.doc2bow(text) for text in texts]
    
    logging.info("term frequent matrix")
    for idx, c in enumerate(corpus):
        print(c) 
    
    logging.info("retweets weights vector: " + str(weight_set.__len__()))
    for w in weight_set:
        print(w);
        
    logging.info("term frequent matrix weighted: " + str(corpus.__len__()))
    for idx, c in enumerate(corpus):
        for i,t in enumerate(c):
            corpus[idx][i]=[t[0], weight_set[idx]]# log(weight_set[idx],2)]  #
        print(c) 
    
    logging.info("getting TF-IDF matrix")
    tfidf = models.TfidfModel(corpus,normalize=False)
    corpus_tfidf = tfidf[corpus]
    
    for idx, c in  enumerate(corpus):
        print(tfidf[c])
        
    print("num topics:" + str(numDocs))
    
    numtopics=numDocs
    numwords=2
    
    logging.info("generating LDA model...")
    ldamodel = gensim.models.ldamodel.LdaModel(corpus_tfidf, num_topics=numtopics, id2word = dictionary, passes=25)
    
    logging.info("generating LSA model...")
    lsimodel = gensim.models.lsimodel.LsiModel(corpus_tfidf, id2word=dictionary, num_topics=numtopics)
    
    logging.info("generating HDP model...")
    hdpmodel = gensim.models.hdpmodel.HdpModel(corpus_tfidf, id2word=dictionary)
    
    
    logging.info("LDA model topics")
    ldamodel.print_topics(num_topics=numtopics, num_words=numwords)
    topics = ldamodel.show_topics(num_topics=numtopics, num_words=numwords, formatted=True)
    
    outputfile = inputfile+".ldatopics"
    ftarget = open(outputfile,'w')
    for t in topics:  
        ftarget.write(t + ', ')
        
    ftarget.close()
    
    logging.info("LSA model topics")
    lsimodel.print_topics(num_topics=numtopics, num_words=numwords)
    topics = lsimodel.show_topics(num_topics=numtopics, num_words=numwords, formatted=True)
    
    outputfile = inputfile+".lsatopics"
    ftarget = open(outputfile,'w')
    for t in topics: 
        ftarget.write(t  + ', ')
    
    ftarget.close()
    
    logging.info("HDP model topics")
    hdpmodel.print_topics(topics=numtopics,topn=numwords)
    
    return texts, dictionary, corpus, tfidf, ldamodel 
    


    
def df2idf(docfreq, totaldocs, log_base=2.0, add=0.0):
    """
    Compute default inverse-document-frequency for a term with document frequency `doc_freq`::

      idf = add + log(totaldocs / doc_freq)
    """
    return add + math.log(1.0 * totaldocs / docfreq, log_base)



from nltk.tokenize import RegexpTokenizer
from stop_words import get_stop_words
from nltk.stem.porter import PorterStemmer
from nltk.stem.snowball import SnowballStemmer
from gensim import corpora, models, similarities
import unicodedata
import unidecode
import gensim
import logging
from math import log
import os.path
import sys

FORMAT = '%(asctime)s %(message)s'
logging.basicConfig(format=FORMAT,level=logging.INFO)
logging.info('initializing experiments...')
language = "english"
dfile =sys.argv[1]
hasWeights = sys.argv[2] if sys.argv.__len__() > 2 else 0
gtfile = sys.argv[3] if sys.argv.__len__() > 3 else None

ttexts, tdic, tcorpus, ttfidf, tlda = processfile(dfile, hasWeights)
#sys.exit()

if gtfile is not None:
    ntexts, ndic, ncorpus, ntfidf, nldf = processfile(gtfile, 0)
    
    print("-----------TWEETS tfidf matrix-------------")
    for idx, c in  enumerate(tcorpus):
        print(ttfidf[c])
    
    print("-----------NEWS tfidf matrix-------------")
    for idx2, c2 in  enumerate(ncorpus):
        print(nldf[c2])
       
    logging.info("text matrix") 
    for t in ttexts:
        print (t)
          
    vcorpus = [ndic.doc2bow(text) for text in ttexts]    
    
    logging.info("term frequent matrix")
    for idx, c in enumerate(vcorpus):
        print(c) 
        
    index = similarities.MatrixSimilarity(ntfidf[ncorpus])
    vec = ntfidf[vcorpus[0]]
    sims = index[vec]
    
    print(list(enumerate(sims)))
    
