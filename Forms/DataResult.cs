using System.Collections.Generic;

namespace Auth.Forms {
    public class DataResult<T> {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
    }
}