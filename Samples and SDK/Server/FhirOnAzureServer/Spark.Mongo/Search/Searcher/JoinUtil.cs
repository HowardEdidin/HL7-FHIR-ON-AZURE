﻿/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */


namespace FhirOnAzure.Search.Mongo
{
    /*
    public class JoinUtil
    {
        // No instances allowed
        private JoinUtil() { }
      
        public static Query CreateJoinQuery(String fromField,
                                            String toField,
                                            Query fromQuery,
                                            IndexSearcher fromSearcher)
        {
            TermsCollector termsCollector = new TermsCollector(fromField);
            fromSearcher.Search(fromQuery, termsCollector);
            return new TermsQuery(toField, termsCollector.GetCollectorTerms());
        }
    }
    */
}


#if JAVA_ORIGINAL
package org.apache.lucene.search.join;

/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

import org.apache.lucene.search.IndexSearcher;
import org.apache.lucene.search.Query;

import java.io.IOException;

/**
 * Utility for query time joining using {@link org.apache.lucene.search.join.TermsQuery} and {@link org.apache.lucene.search.join.TermsCollector}.
 *
 * @lucene.experimental
 */
public final class JoinUtil {

  // No instances allowed
  private JoinUtil() {
  }

  /**
   * Method for query time joining.
   * <p/>
   * Execute the returned query with a {@link org.apache.lucene.search.IndexSearcher} to retrieve all documents that have the same terms in the
   * to field that match with documents matching the specified fromQuery and have the same terms in the from field.
   *
   * Notice: Can't join documents with a fromField that holds more then one term.
   *
   *
   * @param fromField                 The from field to join from
   * @param toField                   The to field to join to
   * @param fromQuery                 The query to match documents on the from side
   * @param fromSearcher              The searcher that executed the specified fromQuery
   * @return a {@link org.apache.lucene.search.Query} instance that can be used to join documents based on the
   *         terms in the from and to field
   * @throws java.io.IOException If I/O related errors occur
   */
  public static Query createJoinQuery(String fromField,
                                      String toField,
                                      Query fromQuery,
                                      IndexSearcher fromSearcher) throws IOException {
    TermsCollector termsCollector = new TermsCollector(fromField);
    fromSearcher.search(fromQuery, termsCollector);
    return new TermsQuery(toField, termsCollector.getCollectorTerms());
  }

}
#endif