json.array!(@test_results) do |test_result|
  json.extract! test_result, :execution_id, :result, :is_triaged, :triaged_by, :description
  json.url test_result_url(test_result, format: :json)
end
