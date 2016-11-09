json.array!(@supported_environments) do |environment|
  json.extract! environment, :id, :name, :description
  json.url build_url(environment, format: :json)
end
