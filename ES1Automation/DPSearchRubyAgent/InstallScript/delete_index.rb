require "elasticsearch" 
require 'hashie'

DPSEARCH_SYSTEM_INDEX = "dpsearch_system"
TYPE="index_config"
begin
  client = Elasticsearch::Client.new
  ##search all indexes, delelte them one by one if exist
  reponse = client.search index: DPSEARCH_SYSTEM_INDEX, type: TYPE

  indexesH = Hashie::Mash.new reponse
  puts '******************************'

  if indexesH != nil && indexesH.hits != nil
      hit_count = indexesH.hits.total.to_i

      if hit_count > 0 then
        index=indexesH.hits.hits
   
        index.each do |i|
            puts 'Delete index: ' + i._source.index_name
            if client.indices.exists index: i._source.index_name then
                client.indices.delete index: i._source.index_name
                puts 'Index deleted : ' +i._source.index_name                
            else
                puts 'Index not existed :'+i._source.index_name
            end
	end
      end
    end
rescue
end
