using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication
{
    interface Reader
    {
        uint read();
        void close() ;

	    bool markSupported() ;

        void setReadLimit(long readLimit) ;
	
	    void mark() ;

	    void reset();

	
	    long skip(long skip) ;

	    long align(long boundary) ;

    }
}
