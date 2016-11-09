json.array!(@test_environments) do |test_environment|
  json.extract! test_environment, :name, :environment_type, :status, :created_at, :modified_at, :config, :description
  json.url test_environment_url(test_environment, format: :json)
end
