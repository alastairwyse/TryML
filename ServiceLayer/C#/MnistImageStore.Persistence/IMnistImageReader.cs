using System;
using System.Collections.Generic;
using System.Text;
using MnistImageStore.Models;

namespace MnistImageStore.Persistence
{
    /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="T:MnistImageStore.Persistence.IMnistImageReader"]/*'/>
    public interface IMnistImageReader
    {
        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MnistImageStore.Persistence.IMnistImageReader.Read"]/*'/>
        IEnumerable<MnistImage> Read();
    }
}
