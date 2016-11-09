require 'elasticsearch'

client = Elasticsearch::Client.new
client.indices.delete index: '_all'
