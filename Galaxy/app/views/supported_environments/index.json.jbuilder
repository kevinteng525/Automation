json.array!(@supported_environments) do |supported_environment|
  json.extract! supported_environment, :name, :description
  json.url supported_environment_url(supported_environment, format: :json)
end
